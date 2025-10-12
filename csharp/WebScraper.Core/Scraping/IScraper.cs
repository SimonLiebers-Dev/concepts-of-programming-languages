using WebScraper.Core.Models;

namespace WebScraper.Core.Scraping;

/// <summary>
/// Defines an interface for scraping a single web page.
/// </summary>
public interface IScraper
{
    /// <summary>
    /// Fetches and parses the specified URL into a <see cref="Page"/> model.
    /// </summary>
    /// <param name="url">The target URL to scrape.</param>
    /// <param name="ct">A cancellation token that allows the operation to be cancelled.</param>
    /// <returns>A <see cref="Page"/> instance representing the scrape result.</returns>
    Task<Page> ScrapeAsync(string url, CancellationToken ct = default);
}