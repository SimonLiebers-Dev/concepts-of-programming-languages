using WebScraper.Core.Fetcher;
using WebScraper.Core.Models;
using WebScraper.Core.Parser;

namespace WebScraper.Core.Scraping;

/// <summary>
/// Default implementation of <see cref="IScraper"/> that combines
/// an <see cref="IHtmlFetcher"/> and <see cref="IHtmlParser"/> to produce a <see cref="Page"/>.
/// </summary>
internal class DefaultScraper : IScraper
{
    private readonly IHtmlFetcher _fetcher;
    private readonly IHtmlParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultScraper"/> class.
    /// </summary>
    /// <param name="fetcher">The HTML fetcher used to retrieve page content.</param>
    /// <param name="parser">The HTML parser used to extract titles and links.</param>
    public DefaultScraper(IHtmlFetcher fetcher, IHtmlParser parser)
    {
        _fetcher = fetcher ?? throw new ArgumentNullException(nameof(fetcher));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    /// <inheritdoc />
    public async Task<Page> ScrapeAsync(string url, CancellationToken ct = default)
    {
        try
        {
            // Fetch HTML
            var html = await _fetcher.FetchAsync(url, ct);

            // Parse HTML
            var result = _parser.Parse(html);

            return Page.SuccessPage(
                url: url,
                title: result.Title,
                links: result.Links,
                images: result.Images,
                timestamp: DateTimeOffset.UtcNow);
        }
        catch (Exception ex)
        {
            return Page.ErrorPage(url, ex.Message, DateTimeOffset.Now);
        }
    }
}