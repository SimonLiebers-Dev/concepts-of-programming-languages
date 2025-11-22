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
    /// Extensions for <see cref="ProgressTask"/>
    /// </summary>
    /// <param name="task"></param>
    extension(ProgressTask task)
    {
        /// <summary>
        /// Marks the specified <see cref="ProgressTask"/> as successfully completed.
        /// </summary>
        /// <param name="url">The URL associated with the operation.</param>
        /// <remarks>
        /// This method sets the task's value to its maximum, stops it from updating further,
        /// and updates the description text to display a green success label.
        /// </remarks>
        public void MarkAsDone(string url)
        {
            task.Value = task.MaxValue;
            task.StopTask();

            task.Description = url.GetDescription("Success", "green");
        }

        /// <summary>
        /// Marks the specified <see cref="ProgressTask"/> as failed and displays an error message.
        /// </summary>
        /// <param name="url">The URL associated with the failed operation.</param>
        /// <remarks>
        /// This method completes the task visually, applies red coloring to indicate failure,
        /// and updates the task description to include an error label and the affected URL.
        /// </remarks>
        public void MarkAsError(string url)
        {
            task.Value = 0;
            task.StopTask();

            task.Description = url.GetDescription("Error", "red");
        }
    }
}