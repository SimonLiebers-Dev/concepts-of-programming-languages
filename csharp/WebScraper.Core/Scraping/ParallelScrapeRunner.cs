using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using WebScraper.Core.Models;
using WebScraper.Core.UI;

namespace WebScraper.Core.Scraping;

/// <summary>
/// <see cref="IScrapeRunner"/> implementation that coordinates sequential and parallel web scraping using the built-in <see cref="Parallel"/> API.
/// </summary>
/// <remarks>
/// This runner uses Parallel.ForEachAsync with configurable
/// <see cref="ParallelOptions.MaxDegreeOfParallelism"/> to control concurrency.
/// When concurrency is less than or equal to zero, the runner allows unlimited parallelism (no throttling).
/// </remarks>
public class ParallelScrapeRunner(
    IScraper scraper,
    ILogger<SemaphoreScrapeRunner> logger,
    IProgressBarManager progressBarManager)
    : BaseScrapeRunner(scraper, logger, progressBarManager), IScrapeRunner
{
    /// <summary>
    /// Executes a sequential scraping process by delegating to
    /// <see cref="RunParallelAsync"/> with a concurrency level of 1.
    /// </summary>
    /// <param name="urls">A collection of URLs to scrape.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to support graceful shutdown.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a list of <see cref="Page"/> results.
    /// </returns>
    public async Task<IReadOnlyList<Page>> RunSequentialAsync(
        IReadOnlyList<string> urls,
        CancellationToken ct = default)
    {
        // Sequential mode is just parallel mode limited to a single worker.
        return await RunParallelAsync(urls, 1, ct);
    }

    /// <summary>
    /// Executes the scraping process in parallel using Parallel.ForEachAsync.
    /// </summary>
    /// <param name="urls">A collection of URLs to scrape.</param>
    /// <param name="concurrency">
    /// The number of concurrent workers. If 0 or negative, concurrency is unbounded.
    /// </param>
    /// <param name="ct">A <see cref="CancellationToken"/> for cooperative cancellation.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a list of <see cref="Page"/> results.
    /// </returns>
    public async Task<IReadOnlyList<Page>> RunParallelAsync(
        IReadOnlyList<string> urls,
        int concurrency,
        CancellationToken ct = default)
    {
        // Wraps the scraping logic with progress rendering lifecycle management
        return await RunWithProgressAsync(urls, async (targets, token) =>
        {
            // ConcurrentBag provides thread-safe collection for storing results
            var results = new ConcurrentBag<Page>();

            // Configure Parallel.ForEachAsync with options
            await Parallel.ForEachAsync(
                targets,
                new ParallelOptions
                {
                    // Limit concurrency to 'concurrency' workers if > 0,
                    // otherwise use -1 (no limit = system decides optimal level)
                    MaxDegreeOfParallelism = concurrency > 0 ? concurrency : -1,
                    CancellationToken = token
                },
                async (url, innerCt) =>
                {
                    // Perform the actual scrape with progress tracking
                    var page = await ScrapeWithTrackingAsync(url, innerCt);

                    // Thread-safe add to result collection
                    results.Add(page);
                });

            // Convert the concurrent collection to a list for return
            return results.ToList();
        }, ct);
    }
}