namespace WebScraper.Core.Fetcher;

/// <summary>
/// Defines an interface for performing HTTP GET requests and retrieving remote HTML content as text.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for making network requests to web resources,
/// applying a configurable <c>User-Agent</c> header, and returning the response body as a string.
/// </remarks>
public interface IHtmlFetcher
{
    /// <summary>
    /// Sets or updates the <c>User-Agent</c> header value that will be sent with subsequent HTTP requests.
    /// </summary>
    /// <param name="userAgent">
    /// The user-agent string to identify the client when making HTTP requests.
    /// If <see langword="null"/> or empty, implementations may revert to a default user agent.
    /// </param>
    void SetUserAgent(string userAgent);

    /// <summary>
    /// Sets the HTTP request timeout for the internal <see cref="HttpClient"/> instance.
    /// </summary>
    /// <param name="httpTimeoutSeconds">
    /// The timeout duration, in seconds, after which an ongoing HTTP request will be aborted.
    /// </param>
    /// <remarks>
    /// This method allows the scraper to dynamically adjust its timeout behavior at runtime,
    /// for example based on configuration values loaded from <c>appsettings.json</c>.
    /// </remarks>
    void SetHttpTimeout(int httpTimeoutSeconds);

    /// <summary>
    /// Performs an asynchronous HTTP GET request for the specified URL and returns the response body as a string.
    /// </summary>
    /// <param name="url">The fully qualified target URL to fetch.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task representing the asynchronous fetch operation.  
    /// On success, the task result contains the raw HTML content returned by the server.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="url"/> parameter is <see langword="null"/> or empty.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Thrown when the HTTP request fails or the server returns a non-success status code.
    /// </exception>
    Task<string> FetchAsync(string url, CancellationToken ct = default);
}