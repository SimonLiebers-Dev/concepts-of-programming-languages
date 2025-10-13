using WebScraper.Core.Models;

namespace WebScraper.Core.Parser;

/// <inheritdoc />
internal class HtmlParser : IHtmlParser
{
    private readonly AngleSharp.Html.Parser.HtmlParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlParser"/> class
    /// used to parse HTML documents.
    /// </summary>
    public HtmlParser()
    {
        _parser = new AngleSharp.Html.Parser.HtmlParser();
    }

    /// <inheritdoc />
    public async Task<ParserResult> ParseAsync(Stream htmlStream, CancellationToken ct = default)
    {
        using var reader = new StreamReader(htmlStream);
        var html = await reader.ReadToEndAsync(ct).ConfigureAwait(false);
        return Parse(html);
    }

    /// <inheritdoc />
    public ParserResult Parse(string html)
    {
        var doc = _parser.ParseDocument(html);
        var title = doc.Title?.Trim() ?? string.Empty;
        var links = doc.QuerySelectorAll("a[href]")
            .Select(a => a.GetAttribute("href"))
            .OfType<string>()
            .Where(href => !string.IsNullOrWhiteSpace(href))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var images = doc.QuerySelectorAll("img[src]")
            .Select(a => a.GetAttribute("src"))
            .OfType<string>()
            .Where(src => !string.IsNullOrWhiteSpace(src))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new ParserResult(title, links, images);
    }
}