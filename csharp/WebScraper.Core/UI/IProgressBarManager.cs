using Spectre.Console;

namespace WebScraper.Core.UI;

/// <summary>
/// Defines a interface for managing and rendering multiple <see cref="ProgressTask"/> instances
/// using <see cref="Spectre.Console"/> progress bars.
/// </summary>
/// <remarks>
/// This interface abstracts the lifecycle of progress bar rendering within the console UI,
/// including initialization, task creation, and graceful termination.
/// </remarks>
public interface IProgressBarManager : IAsyncDisposable
{
    /// <summary>
    /// Starts rendering progress bars asynchronously in the console.
    /// </summary>
    /// <remarks>
    /// This method must be called before creating or updating any progress tasks.
    /// Typically, the rendering loop runs until <see cref="StopRendererAsync"/> is invoked.
    /// </remarks>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation of starting
    /// the progress display loop.
    /// </returns>
    Task StartRenderingAsync();

    /// <summary>
    /// Creates a new <see cref="ProgressTask"/> with the specified name and maximum value.
    /// </summary>
    /// <param name="url">A descriptive label, typically the URL or task identifier.</param>
    /// <param name="maxValue">The maximum progress value that indicates completion.</param>
    /// <returns>
    /// A new <see cref="ProgressTask"/> instance that can be updated incrementally
    /// to reflect task progress.
    /// </returns>
    ProgressTask CreateProgressBar(string url, int maxValue);

    /// <summary>
    /// Stops rendering all active progress bars and finalizes the display output.
    /// </summary>
    /// <remarks>
    /// This method signals the rendering loop to complete gracefully.
    /// It should be called after all tasks have finished or when cancellation occurs.
    /// </remarks>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous cleanup operation.
    /// </returns>
    ValueTask StopRendererAsync();
}