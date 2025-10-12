using System.Text.Json.Serialization;

namespace WebScraper.Core.Models;

public sealed record Page(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("links")] IReadOnlyList<string> Links,
    [property: JsonPropertyName("timestamp")]
    DateTimeOffset Timestamp,
    [property: JsonPropertyName("error")] string? Error)
{
    [JsonIgnore] public bool HasError => !string.IsNullOrEmpty(Error);

    [JsonIgnore] public bool Success => string.IsNullOrEmpty(Error);

    public static Page SuccessPage(string url, string? title, IEnumerable<string>? links = null,
        DateTimeOffset? timestamp = null)
        => new(url, title, links?.ToArray() ?? [], timestamp ?? DateTimeOffset.UtcNow, null);

    public static Page ErrorPage(string url, string error, DateTimeOffset? timestamp = null)
        => new(url, null, [], timestamp ?? DateTimeOffset.UtcNow, error);
}