using WebScraper.Core.Models;

namespace WebScraper.Core.Scraping;

/// <summary>
/// Defines an interface for coordinating sequential and parallel web scraping operations.
/// </summary>
/// <remarks>
/// An <see cref="IScrapeRunner"/> implementation orchestrates the scraping workflow by delegating
/// URL processing to individual <see cref="IScraper"/> instances and managing concurrency,
/// progress tracking, and cancellation support.
/// </remarks>
public interface IScrapeRunner
{
    /// <summary>
    /// Executes a sequential scraping process, fetching and parsing each URL one after another.
    /// </summary>
    /// <param name="urls">A collection of target URLs to scrape.</param>
    /// <param name="ct">
    /// A <see cref="CancellationToken"/> used to cancel the operation gracefully.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, returning a collection of
    /// <see cref="Page"/> results for all processed URLs.
    /// </returns>
    Task<IReadOnlyList<Page>> RunSequentialAsync(
        IReadOnlyList<string> urls,
        CancellationToken ct = default);

    /// <summary>
    /// Executes a parallel scraping process using a configurable worker pool.
    /// </summary>
    /// <param name="urls">A collection of target URLs to scrape.</param>
    /// <param name="concurrency">
    /// The number of concurrent workers to run in parallel.
    /// Must be greater than zero; implementations may adjust invalid values automatically.
    /// </param>
    /// <param name="ct">
    /// A <see cref="CancellationToken"/> used to cancel all running operations gracefully.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, returning a collection of
    /// <see cref="Page"/> results for all processed URLs.
    /// </returns>
    Task<IReadOnlyList<Page>> RunParallelAsync(
        IReadOnlyList<string> urls,
        int concurrency,
        CancellationToken ct = default);
}