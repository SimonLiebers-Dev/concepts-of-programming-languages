using Microsoft.Extensions.Logging;
using Spectre.Console;
using WebScraper.Core.Extensions;
using WebScraper.Core.Models;
using WebScraper.Core.UI;

namespace WebScraper.Core.Scraping;

/// <summary>
/// Coordinates sequential and parallel web scraping, integrating
/// the scraper logic with progress tracking and cancellation.
/// </summary>
internal class DefaultScrapeRunner : IScrapeRunner
{
    private readonly IScraper _scraper;
    private readonly ILogger<DefaultScrapeRunner> _logger;
    private readonly IProgressBarManager _progressBarManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultScrapeRunner"/> class.
    /// </summary>
    /// <param name="scraper">The scraper responsible for fetching and parsing pages.</param>
    /// <param name="logger">The logger used for diagnostic output.</param>
    /// <param name="progressBarManager">A factory that creates <see cref="IProgressBarManager"/> instances.</param>
    public DefaultScrapeRunner(IScraper scraper, ILogger<DefaultScrapeRunner> logger,
        IProgressBarManager progressBarManager)
    {
        _scraper = scraper ?? throw new ArgumentNullException(nameof(scraper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressBarManager = progressBarManager ?? throw new ArgumentNullException(nameof(progressBarManager));
    }

    /// <summary>
    /// Runs the scraper sequentially, scraping each URL one after another while updating the progress display.
    /// </summary>
    /// <param name="urls">The list of URLs to scrape.</param>
    /// <param name="ct">A cancellation token to support graceful shutdown.</param>
    /// <returns>A list of <see cref="Page"/> results.</returns>
    public async Task<IReadOnlyList<Page>> RunSequentialAsync(
        IReadOnlyList<string> urls,
        CancellationToken ct = default)
    {
        await _progressBarManager.StartRenderingAsync();

        var results = new List<Page>(urls.Count);

        try
        {
            foreach (var url in urls)
            {
                if (ct.IsCancellationRequested)
                    break;

                var tracker = _progressBarManager.CreateProgressBar(url, 2);
                tracker.Increment(1); // started

                var page = await _scraper.ScrapeAsync(url, ct);
                if (page.Success)
                {
                    tracker.MarkAsDone(url);
                }
                else
                {
                    tracker.MarkAsError(url);
                }

                results.Add(page);
            }
        }
        finally
        {
            await _progressBarManager.StopRendererAsync();
        }

        return results;
    }

    /// <summary>
    /// Runs the scraper concurrently using a configurable worker pool.
    /// </summary>
    /// <param name="urls">The list of URLs to scrape.</param>
    /// <param name="concurrency">The number of concurrent workers (minimum 1).</param>
    /// <param name="ct">A cancellation token to support graceful shutdown.</param>
    /// <returns>A list of <see cref="Page"/> results.</returns>
    public async Task<IReadOnlyList<Page>> RunParallelAsync(
        IReadOnlyList<string> urls,
        int concurrency,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Starting parallel scrape...");
        if (concurrency <= 0)
            concurrency = 1;

        await _progressBarManager.StartRenderingAsync();

        var results = new List<Page>(urls.Count);
        var semaphore = new SemaphoreSlim(concurrency);
        var tasks = new List<Task>();

        try
        {
            foreach (var url in urls)
            {
                await semaphore.WaitAsync(ct).ConfigureAwait(false);

                tasks.Add(Task.Run(async () =>
                {
                    var tracker = _progressBarManager.CreateProgressBar(url, 2);
                    tracker.Increment(1); // started

                    var page = await _scraper.ScrapeAsync(url, ct).ConfigureAwait(false);
                    if (page.Success)
                    {
                        tracker.MarkAsDone(url);
                    }
                    else
                    {
                        tracker.MarkAsError(url);
                    }

                    lock (results)
                        results.Add(page);

                    semaphore.Release();
                }, ct));
            }

            // Wait for all scraping tasks to complete
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        finally
        {
            await _progressBarManager.StopRendererAsync();
        }

        return results;
    }
}