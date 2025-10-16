using Microsoft.Extensions.Logging;
using WebScraper.Core.Models;
using WebScraper.Core.UI;

namespace WebScraper.Core.Scraping;

/// <summary>
/// Coordinates sequential and parallel web scraping, integrating
/// the scraper logic with progress tracking and cancellation.
/// </summary>
/// <remarks>
/// This implementation uses a <see cref="SemaphoreSlim"/> to limit
/// concurrency during parallel scraping. Each worker acquires a semaphore
/// slot before scraping a URL and releases it upon completion.
/// </remarks>
public class SemaphoreScrapeRunner : BaseScrapeRunner
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SemaphoreScrapeRunner"/> class.
    /// </summary>
    /// <param name="scraper">The scraper responsible for fetching and parsing pages.</param>
    /// <param name="logger">The logger used for diagnostic output.</param>
    /// <param name="progressBarManager">A factory that creates <see cref="IProgressBarManager"/> instances.</param>
    public SemaphoreScrapeRunner(
        IScraper scraper,
        ILogger<SemaphoreScrapeRunner> logger,
        IProgressBarManager progressBarManager)
        : base(scraper, logger, progressBarManager)
    {
    }

    /// <summary>
    /// Runs the scraper concurrently using a configurable worker pool.
    /// </summary>
    /// <remarks>
    /// This version uses a <see cref="SemaphoreSlim"/> to control the number of
    /// concurrent scraping tasks. Each worker waits for an available slot before
    /// scraping and releases the slot once finished.
    /// </remarks>
    public override async Task<IReadOnlyList<Page>> RunParallelAsync(
        IReadOnlyList<string> urls,
        int concurrency,
        CancellationToken ct = default)
    {
        // Ensure minimum concurrency level of 1
        if (concurrency <= 0)
            concurrency = 1;

        return await RunWithProgressAsync(urls, async (targets, token) =>
        {
            var results = new List<Page>(targets.Count);
            var semaphore = new SemaphoreSlim(concurrency);
            var tasks = new List<Task>();

            foreach (var url in targets)
            {
                // Wait for an available semaphore slot before starting a new task
                await semaphore.WaitAsync(token).ConfigureAwait(false);

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        // Use shared helper for consistent progress tracking and error handling
                        var page = await ScrapeWithTrackingAsync(url, token).ConfigureAwait(false);

                        lock (results)
                            results.Add(page);
                    }
                    finally
                    {
                        // Always release the semaphore, even on error or cancellation
                        semaphore.Release();
                    }
                }, token));
            }

            // Wait for all worker tasks to complete
            await Task.WhenAll(tasks).ConfigureAwait(false);

            return results;
        }, ct);
    }
}