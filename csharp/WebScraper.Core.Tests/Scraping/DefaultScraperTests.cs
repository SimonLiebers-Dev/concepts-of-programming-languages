using Moq;
using WebScraper.Core.Fetcher;
using WebScraper.Core.Models;
using WebScraper.Core.Parser;
using WebScraper.Core.Scraping;

namespace WebScraper.Core.Tests.Scraping;

[TestFixture]
public class DefaultScraperTests
{
    private Mock<IHtmlFetcher> _fetcherMock = null!;
    private Mock<IHtmlParser> _parserMock = null!;
    private DefaultScraper _scraper = null!;

    [SetUp]
    public void SetUp()
    {
        _fetcherMock = new Mock<IHtmlFetcher>();
        _parserMock = new Mock<IHtmlParser>();
        _scraper = new DefaultScraper(_fetcherMock.Object, _parserMock.Object);
    }

    [Test]
    public async Task ScrapeAsync_ShouldReturnSuccessPage_WhenFetchAndParseSucceed()
    {
        // Arrange
        const string url = "https://example.com";
        const string html = "<html><title>Example</title></html>";

        var parserResult = new ParserResult("Example", ["link1", "link2"], ["img1", "img2"]);

        _fetcherMock
            .Setup(f => f.FetchAsync(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(html);

        _parserMock
            .Setup(p => p.Parse(html))
            .Returns(parserResult);

        // Act
        var page = await _scraper.ScrapeAsync(url);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(page.Success, Is.True);
            Assert.That(page.Url, Is.EqualTo(url));
            Assert.That(page.Title, Is.EqualTo("Example"));
            Assert.That(page.Links, Is.EquivalentTo((string[])["link1", "link2"]));
            Assert.That(page.Images, Is.EquivalentTo((string[])["img1", "img2"]));
            Assert.That(page.ErrorMessage, Is.Null);
            Assert.That(page.Timestamp, Is.Not.EqualTo(default(DateTimeOffset)));
        });
    }

    [Test]
    public async Task ScrapeAsync_ShouldReturnErrorPage_WhenFetchThrows()
    {
        // Arrange
        const string url = "https://fail.com";
        _fetcherMock
            .Setup(f => f.FetchAsync(url, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var page = await _scraper.ScrapeAsync(url);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(page.Success, Is.False);
            Assert.That(page.ErrorMessage, Does.Contain("Network error"));
            Assert.That(page.Url, Is.EqualTo(url));
            Assert.That(page.Title, Is.Null);
            Assert.That(page.Links, Is.Empty);
            Assert.That(page.Images, Is.Empty);
        });
    }

    [Test]
    public async Task ScrapeAsync_ShouldReturnErrorPage_WhenParserThrows()
    {
        // Arrange
        const string url = "https://broken.com";
        const string html = "<html><broken></html>";

        _fetcherMock
            .Setup(f => f.FetchAsync(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(html);

        _parserMock
            .Setup(p => p.Parse(html))
            .Throws(new InvalidOperationException("Parsing failed"));

        // Act
        var page = await _scraper.ScrapeAsync(url);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(page.Success, Is.False);
            Assert.That(page.ErrorMessage, Does.Contain("Parsing failed"));
            Assert.That(page.Url, Is.EqualTo(url));
            Assert.That(page.Links, Is.Empty);
            Assert.That(page.Images, Is.Empty);
        });
    }

    [Test]
    public void Constructor_ShouldThrow_WhenFetcherIsNull()
    {
        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => { _ = new DefaultScraper(null!, _parserMock.Object); });

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("fetcher"));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenParserIsNull()
    {
        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => { _ = new DefaultScraper(_fetcherMock.Object, null!); });

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("parser"));
    }
}