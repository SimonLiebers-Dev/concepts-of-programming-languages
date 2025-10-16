using System.Globalization;

namespace WebScraper.Core.Extensions;

/// <summary>
/// Provides extension methods for formatting <see cref="TimeSpan"/> values
/// into human-readable strings.
/// </summary>
public static class TimeSpanExtensions
{
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> into a readable text format.
    /// 
    /// The output adapts dynamically based on the duration:
    /// <list type="bullet">
    /// <item>
    /// <description>Under 1 second → displays as milliseconds (e.g. <c>350ms</c>)</description>
    /// </item>
    /// <item>
    /// <description>Under 1 minute → displays as seconds (e.g. <c>12.5s</c>)</description>
    /// </item>
    /// <item>
    /// <description>Under 1 hour → displays as minutes and seconds (e.g. <c>2m 05s</c>)</description>
    /// </item>
    /// <item>
    /// <description>Under 1 day → displays as hours and minutes (e.g. <c>3h 42m</c>)</description>
    /// </item>
    /// <item>
    /// <description>1 day or more → displays as days and hours (e.g. <c>1d 04h</c>)</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="ts">The <see cref="TimeSpan"/> instance to format.</param>
    /// <param name="culture">
    /// Optional culture for number formatting.  
    /// Defaults to <see cref="CultureInfo.InvariantCulture"/>.
    /// </param>
    /// <returns>
    /// A human-friendly string representation of the time span
    /// (e.g. <c>450ms</c>, <c>12.3s</c>, <c>2m 10s</c>, <c>3h 42m</c>, <c>1d 4h</c>).
    /// </returns>
    public static string ToFormattedString(this TimeSpan ts, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.InvariantCulture;

        if (ts.TotalMilliseconds < 1000)
            return string.Format(culture, "{0:F0}ms", ts.TotalMilliseconds);

        if (ts.TotalSeconds < 60)
            return string.Format(culture, "{0:F1}s", ts.TotalSeconds);

        if (ts.TotalMinutes < 60)
            return string.Format(culture, "{0}m {1:D2}s", (int)ts.TotalMinutes, ts.Seconds);

        return ts.TotalHours < 24
            ? string.Format(culture, "{0}h {1:D2}m", (int)ts.TotalHours, ts.Minutes)
            : string.Format(culture, "{0}d {1:D2}h", (int)ts.TotalDays, ts.Hours);
    }
}