using System.Text.Json.Serialization;

namespace WebScraper.Core.Models;

/// <summary>
/// Represents the result of a single web scraping operation.
/// </summary>
/// <remarks>
/// A <see cref="Page"/> captures metadata about a scraped webpage, including its URL,
/// title, extracted links, timestamp, and success or failure state.  
/// It is immutable and can be serialized to JSON for reporting.
/// </remarks>
public record Page(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("links")] IReadOnlyList<string> Links,
    [property: JsonPropertyName("images")] IReadOnlyList<string> Images,
    [property: JsonPropertyName("timestamp")]
    DateTimeOffset Timestamp,
    [property: JsonPropertyName("success")]
    bool Success,
    [property: JsonPropertyName("errorMessage")]
    string? ErrorMessage)
{
    /// <summary>
    /// Creates a new <see cref="Page"/> instance representing a successful scrape.
    /// </summary>
    /// <param name="url">The URL of the successfully scraped page.</param>
    /// <param name="title">The extracted HTML title of the page.</param>
    /// <param name="links">An optional collection of hyperlinks found on the page.</param>
    /// <param name="images">An optional collection of images found on the page.</param>
    /// <param name="timestamp">
    /// The time the page was scraped.  
    /// </param>
    /// <returns>A <see cref="Page"/> instance with <see cref="Success"/> set to <see langword="true"/>.</returns>
    public static Page SuccessPage(string url, string? title, IEnumerable<string>? links = null,
        IEnumerable<string>? images = null,
        DateTimeOffset? timestamp = null)
        => new(url, title, links?.ToArray() ?? [], images?.ToArray() ?? [], timestamp ?? DateTimeOffset.UtcNow, true,
            null);

    /// <summary>
    /// Creates a new <see cref="Page"/> instance representing a failed scrape.
    /// </summary>
    /// <param name="url">The URL that failed to scrape.</param>
    /// <param name="error">A descriptive error message explaining the failure.</param>
    /// <param name="timestamp">
    /// The time the scrape attempt occurred.
    /// </param>
    /// <returns>A <see cref="Page"/> instance with <see cref="Success"/> set to <see langword="false"/>.</returns>
    public static Page ErrorPage(string url, string error, DateTimeOffset? timestamp = null)
        => new(url, null, [], [], timestamp ?? DateTimeOffset.UtcNow, false, error);
}