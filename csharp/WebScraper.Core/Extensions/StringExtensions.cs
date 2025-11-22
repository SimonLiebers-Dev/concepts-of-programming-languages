using System.Globalization;

namespace WebScraper.Core.Extensions;

/// <summary>
/// Provides utility methods for strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Extensions for type <see cref="string"/>
    /// </summary>
    /// <param name="s">String to use in extension</param>
    extension(string s)
    {
        /// <summary>
        /// Get display width of a string.
        /// This method is helpful for icons with different display widths.
        /// </summary>
        /// <returns>The display width of a string</returns>
        public int GetDisplayWidth()
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
        /// <param name="totalWidth">Total width of the string</param>
        /// <returns></returns>
        public string PadDisplayRight(int totalWidth)
        {
            var displayWidth = GetDisplayWidth(s);
            var spacesToAdd = Math.Max(0, totalWidth - displayWidth);
            return s + new string(' ', spacesToAdd);
        }

        /// <summary>
        /// Truncates a string to maximum length
        /// and adding truncation indicator behind it
        /// </summary>
        /// <param name="maxLength">Maximum length of the string including truncation indicator</param>
        /// <param name="truncationIndicator">Truncation indicator included after truncated text. The default is "...".</param>
        /// <returns></returns>
        public string TruncateText(int maxLength, string truncationIndicator = "...")
        {
            return s.Length <= maxLength ? s : $"{s[..(maxLength - truncationIndicator.Length)]}{truncationIndicator}";
        }

        /// <summary>
        /// Returns a string for the description of a progress bar
        /// </summary>
        /// <param name="status">Status displayed in front</param>
        /// <param name="color">Color of the text</param>
        /// <returns></returns>
        public string GetDescription(string status, string color)
        {
            return $"[{color}]{status,-10}{EscapeText(s)}[/]";
        }

        /// <summary>
        /// Removes characters from strings that could break the console output
        /// and make sure the content strings length matches the provided parameter
        /// </summary>
        /// <param name="length">Length of the text to be returned</param>
        /// <returns></returns>
        private string EscapeText(int length = 50)
        {
            var result = s
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("\t", "");

            // First truncate the text to length, then ensure that the length matches
            return result.TruncateText(length).PadRight(length);
        }
    }
}