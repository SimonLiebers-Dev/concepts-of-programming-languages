namespace WebScraper.Core.Models;

/// <summary>
/// Represents the result of parsing an HTML page.
/// </summary>
public sealed record ParserResult(
    string Title,
    string[] Links);