using System.Text;
using WebScraper.Core.Parser;

namespace WebScraper.Core.Tests.Parser;

[TestFixture]
public class HtmlParserTests
{
    private IHtmlParser _parser = null!;

    [SetUp]
    public void SetUp()
    {
        _parser = new HtmlParser();
    }

    [Test]
    public void Parse_ShouldExtractTitleLinksAndImages()
    {
        // Arrange
        const string html = """
                                <html>
                                    <head><title>Test Page</title></head>
                                    <body>
                                        <a href="https://example.com">Example</a>
                                        <a href="https://example.com">Duplicate</a>
                                        <a href="https://other.com">Other</a>
                                        <img src="image1.png" />
                                        <img src="image2.png" />
                                        <img src="image1.png" />
                                    </body>
                                </html>
                            """;

        // Act
        var result = _parser.Parse(html);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Title, Is.EqualTo("Test Page"));
            Assert.That(result.Links, Is.EquivalentTo((string[])["https://example.com", "https://other.com"]));
            Assert.That(result.Images, Is.EquivalentTo((string[])["image1.png", "image2.png"]));
        });
    }

    [Test]
    public void Parse_ShouldReturnEmptyValues_WhenHtmlIsEmpty()
    {
        // Act
        var result = _parser.Parse(string.Empty);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Title, Is.Empty);
            Assert.That(result.Links, Is.Empty);
            Assert.That(result.Images, Is.Empty);
        });
    }

    [Test]
    public void Parse_ShouldIgnoreEmptyAndWhitespaceLinks()
    {
        // Arrange
        const string html = """
                                <html><body>
                                    <a href="">Empty</a>
                                    <a href="   ">Whitespace</a>
                                    <a href="https://example.com">Valid</a>
                                </body></html>
                            """;

        // Act
        var result = _parser.Parse(html);

        // Assert
        Assert.That(result.Links, Is.EqualTo((string[])["https://example.com"]));
    }

    [Test]
    public void Parse_ShouldHandleNoTitleGracefully()
    {
        // Arrange
        const string html = "<html><body><p>No title here</p></body></html>";

        // Act
        var result = _parser.Parse(html);

        // Assert
        Assert.That(result.Title, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Parse_ShouldBeCaseInsensitiveForLinksAndImages()
    {
        // Arrange
        const string html = """
                                <html><body>
                                    <a href="HTTPS://EXAMPLE.COM">Upper</a>
                                    <a href="https://example.com">Lower</a>
                                    <img src="IMAGE.PNG" />
                                    <img src="image.png" />
                                </body></html>
                            """;

        // Act
        var result = _parser.Parse(html);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Links, Has.Length.EqualTo(1));
            Assert.That(result.Images, Has.Length.EqualTo(1));
        });
    }
}