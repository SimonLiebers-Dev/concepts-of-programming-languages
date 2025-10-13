namespace WebScraper.Cli.App;

/// <summary>
/// Represents an interface for the top-level application entry point.
/// Handles user interaction, orchestrates scraping, prints summaries,
/// and optionally persists the results.
/// </summary>
public interface IApplication
{
    /// <summary>
    /// Executes the main asynchronous workflow of the CLI scraper.
    /// </summary>
    /// <param name="ct">
    /// A <see cref="CancellationToken"/> that can be used to stop the operation gracefully.  
    /// Typically propagated from the host environment or the user.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution of the CLI application.  
    /// The task completes once scraping and output operations have finished.
    /// </returns>
    Task RunAsync(CancellationToken ct = default);
}