using System.Globalization;

namespace WebScraper.Core.Extensions;

/// <summary>
/// Provides utility methods for strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Get display width of a string.
    /// This method is helpful for icons with different display widths.
    /// </summary>
    /// <param name="s">String to check</param>
    /// <returns>The display width of a string</returns>
    public static int GetDisplayWidth(this string s)
    {
        var width = 0;
        var enumerator = StringInfo.GetTextElementEnumerator(s);

        while (enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();

            // Emojis are "wide", others are narrow
            width += element.Length > 1 ? 2 : 1;
        }

        return width;
    }

    /// <summary>
    /// Returns a new string that left-aligns the characters in this string by padding them with spaces on the right, for a specified total width.
    /// </summary>
    /// <param name="s">String to apply padding to</param>
    /// <param name="totalWidth">Total width of the string</param>
    /// <returns></returns>
    public static string PadDisplayRight(this string s, int totalWidth)
    {
        var displayWidth = GetDisplayWidth(s);
        var spacesToAdd = Math.Max(0, totalWidth - displayWidth);
        return s + new string(' ', spacesToAdd);
    }

    /// <summary>
    /// Truncates a string to maximum length
    /// and adding truncation indicator behind it
    /// </summary>
    /// <param name="s">String to truncate</param>
    /// <param name="maxLength">Maximum length of the string including truncation indicator</param>
    /// <param name="truncationIndicator">Truncation indicator included after truncated text. The default is "...".</param>
    /// <returns></returns>
    public static string TruncateText(this string s, int maxLength, string truncationIndicator = "...")
    {
        return s.Length <= maxLength ? s : $"{s[..(maxLength - truncationIndicator.Length)]}{truncationIndicator}";
    }
}