using Microsoft.Extensions.Logging;
using WebScraper.Core.Extensions;
using WebScraper.Core.Models;
using WebScraper.Core.UI;

namespace WebScraper.Core.Scraping;

/// <summary>
/// Provides shared functionality for coordinating web scraping operations,
/// including progress tracking, logging, and cancellation handling.
/// </summary>
/// <remarks>
/// This abstract base class encapsulates common behaviors for all scrape runners,
/// such as progress rendering lifecycle management and tracker updates.
/// </remarks>
public abstract class BaseScrapeRunner : IScrapeRunner
{
    /// <summary>
    /// Logger
    /// </summary>
    protected readonly ILogger Logger;

    private readonly IScraper _scraper;
    private readonly IProgressBarManager _progressBarManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseScrapeRunner"/> class.
    /// </summary>
    /// <param name="scraper">The <see cref="IScraper"/> responsible for fetching and parsing pages.</param>
    /// <param name="logger">The logger instance for recording diagnostic messages.</param>
    /// <param name="progressBarManager">
    /// The <see cref="IProgressBarManager"/> responsible for creating and rendering progress bars.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
    protected BaseScrapeRunner(
        IScraper scraper,
        ILogger logger,
        IProgressBarManager progressBarManager)
    {
        _scraper = scraper ?? throw new ArgumentNullException(nameof(scraper));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressBarManager = progressBarManager ?? throw new ArgumentNullException(nameof(progressBarManager));
    }

    /// <summary>
    /// Wraps a scraping process with progress bar lifecycle management.
    /// </summary>
    protected async Task<IReadOnlyList<Page>> RunWithProgressAsync(
        IReadOnlyList<string> urls,
        Func<IReadOnlyList<string>, CancellationToken, Task<IReadOnlyList<Page>>> scrapeFunc,
        CancellationToken ct)
    {
        // If the caller passed an already-canceled token, return empty results
        if (ct.IsCancellationRequested)
            return [];

        await _progressBarManager.StartRenderingAsync();
        try
        {
            return await scrapeFunc(urls, ct);
        }
        finally
        {
            await _progressBarManager.StopRendererAsync();
        }
    }

    /// <summary>
    /// Executes a single URL scrape and updates the progress tracker accordingly.
    /// </summary>
    protected async Task<Page> ScrapeWithTrackingAsync(string url, CancellationToken ct)
    {
        var tracker = _progressBarManager.CreateProgressBar(url, 2);
        tracker.Increment(1); // mark as started

        var page = await _scraper.ScrapeAsync(url, ct).ConfigureAwait(false);
        if (page.Success)
            tracker.MarkAsDone(url);
        else
            tracker.MarkAsError(url);

        return page;
    }


    /// <inheritdoc />
    public Task<IReadOnlyList<Page>> RunSequentialAsync(IReadOnlyList<string> urls,
        CancellationToken ct = default) => RunParallelAsync(urls, 1, ct);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<Page>> RunParallelAsync(IReadOnlyList<string> urls, int concurrency,
        CancellationToken ct = default);
}