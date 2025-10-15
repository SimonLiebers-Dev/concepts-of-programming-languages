using Microsoft.Extensions.Logging;

namespace WebScraper.Core.Fetcher;

/// <inheritdoc />
public class HtmlFetcher(HttpClient httpClient, ILogger<HtmlFetcher> logger) : IHtmlFetcher
{
    /// <summary>
    /// The default user-agent string used when none is explicitly set.
    /// </summary>
    private const string DefaultUserAgent =
        "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X) " +
        "AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.6.2 " +
        "Mobile/15E148 Safari/604.1";

    private string _userAgent = DefaultUserAgent;

    /// <inheritdoc />
    public void SetUserAgent(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            logger.LogWarning("Empty user-agent provided. Reverting to default.");
            _userAgent = DefaultUserAgent;
        }
        else
        {
            _userAgent = userAgent.Trim();
        }
    }

    /// <inheritdoc />
    public void SetHttpTimeout(int httpTimeoutSeconds)
    {
        // Fallback to 30 seconds if value is not bigger than 0
        if (httpTimeoutSeconds <= 0)
            httpTimeoutSeconds = 30;

        httpClient.Timeout = TimeSpan.FromSeconds(httpTimeoutSeconds);
        logger.LogInformation("HTTP timeout set to {TimeoutSeconds} seconds.", httpTimeoutSeconds);
    }

    /// <inheritdoc />
    public async Task<string> FetchAsync(string url, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd(_userAgent);

        try
        {
            logger.LogDebug("Fetching {Url} ...", url);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, ct)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var msg = $"Unexpected status {(int)response.StatusCode} {response.ReasonPhrase ?? ""}";
                logger.LogWarning("{Url} -> {StatusCode} {Reason}", url, (int)response.StatusCode,
                    response.ReasonPhrase);
                throw new HttpRequestException(msg);
            }

            var html = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            logger.LogDebug("{Url} fetched successfully ({Length} chars)", url, html.Length);
            return html;
        }
        catch (TaskCanceledException) when (ct.IsCancellationRequested)
        {
            logger.LogInformation("Fetch cancelled for {Url}", url);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch {Url}", url);
            throw;
        }
    }
}