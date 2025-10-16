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

        task.Description = GetDescription("Success", "green", url);
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
        task.Value = 0;
        task.StopTask();
        
        task.Description = GetDescription("Error", "red", url);
    }

    /// <summary>
    /// Returns a string for the description of a progress bar
    /// </summary>
    /// <param name="status">Status displayed in front</param>
    /// <param name="color">Color of the text</param>
    /// <param name="content">Description content displayed after status</param>
    /// <returns></returns>
    public static string GetDescription(string status, string color, string content)
    {
        return $"[{color}]{status,-10}{EscapeText(content)}[/]";
    }

    /// <summary>
    /// Removes characters from strings that could break the console output
    /// and make sure the content strings length matches the provided parameter
    /// </summary>
    /// <param name="text">The text to be escaped and shortened</param>
    /// <param name="length">Length of the text to be returned</param>
    /// <returns></returns>
    private static string EscapeText(this string text, int length = 50)
    {
        var result = text
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace("\t", "");

        // First truncate the text to length, then ensure that the length matches
        return result.TruncateText(length).PadRight(length);
    }
}