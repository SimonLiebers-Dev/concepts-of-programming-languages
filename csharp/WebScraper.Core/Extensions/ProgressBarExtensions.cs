using Spectre.Console;

namespace WebScraper.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ProgressTask"/> to simplify
/// marking tasks as completed or errored within Spectre.Console progress displays.
/// </summary>
/// <remarks>
/// These helpers standardize task state handling across the application,
/// making progress output more expressive and consistent in the scraper's UI.
/// </remarks>
public static class ProgressBarExtensions
{
    /// <summary>
    /// Marks the specified <see cref="ProgressTask"/> as successfully completed.
    /// </summary>
    /// <param name="task">The task to mark as complete.</param>
    /// <param name="url">The URL associated with the operation.</param>
    /// <remarks>
    /// This method sets the task's value to its maximum, stops it from updating further,
    /// and updates the description text to display a green success label.
    /// </remarks>
    public static void MarkAsDone(this ProgressTask task, string url)
    {
        task.Value = task.MaxValue;
        task.StopTask();

        task.Description = $"[green]Success {url}[/]";
    }

    /// <summary>
    /// Marks the specified <see cref="ProgressTask"/> as failed and displays an error message.
    /// </summary>
    /// <param name="task">The task to mark as failed.</param>
    /// <param name="url">The URL associated with the failed operation.</param>
    /// <remarks>
    /// This method completes the task visually, applies red coloring to indicate failure,
    /// and updates the task description to include an error label and the affected URL.
    /// </remarks>
    public static void MarkAsError(this ProgressTask task, string url)
    {
        task.Value = task.MaxValue;
        task.StopTask();

        task.Description = $"[red]Error {url}[/]";
    }
}