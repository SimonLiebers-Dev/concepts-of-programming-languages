using WebScraper.Core.Models;

namespace WebScraper.Core.Scraping;

public interface IScrapeRunner
{
    Task<IReadOnlyList<Page>> RunSequentialAsync(
        IReadOnlyList<string> urls,
        CancellationToken ct = default);

    Task<IReadOnlyList<Page>> RunParallelAsync(
        IReadOnlyList<string> urls,
        int concurrency,
        CancellationToken ct = default);
}