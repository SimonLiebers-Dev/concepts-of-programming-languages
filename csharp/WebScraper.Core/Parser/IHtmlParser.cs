using WebScraper.Core.Models;

namespace WebScraper.Core.Parser;

/// <summary>
/// Defines an interface for parsing HTML content into a structured <see cref="ParserResult"/>.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for extracting  data (such as titles and links)
/// from raw HTML documents.
/// </remarks>
public interface IHtmlParser
{
    /// <summary>
    /// Parses HTML content from a <see cref="string"/> and returns a <see cref="ParserResult"/>.
    /// </summary>
    /// <param name="html">The HTML content as a string.</param>
    /// <returns>The parsed result containing the page title, discovered links and images</returns>
    ParserResult Parse(string html);
}