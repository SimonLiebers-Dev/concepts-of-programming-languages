using Microsoft.Extensions.Logging;
using Moq;
using Spectre.Console;
using WebScraper.Core.Models;
using WebScraper.Core.Scraping;
using WebScraper.Core.UI;

namespace WebScraper.Core.Tests.Scraping;

[TestFixture]
public class SemaphoreScrapeRunnerTests
{
    private Mock<IScraper> _scraperMock = null!;
    private Mock<ILogger<SemaphoreScrapeRunner>> _loggerMock = null!;
    private Mock<IProgressBarManager> _progressMock = null!;
    private SemaphoreScrapeRunner _runner = null!;

    [SetUp]
    public void SetUp()
    {
        _scraperMock = new Mock<IScraper>();
        _loggerMock = new Mock<ILogger<SemaphoreScrapeRunner>>();
        _progressMock = new Mock<IProgressBarManager>();

        _runner = new SemaphoreScrapeRunner(_scraperMock.Object, _loggerMock.Object, _progressMock.Object);
    }

    [Test]
    public async Task RunSequentialAsync_ShouldStopWhenCancelled()
    {
        // Arrange
        var urls = new[] { "url1", "url2" };
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var result = await _runner.RunSequentialAsync(urls, cts.Token);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Constructor_ShouldThrow_WhenScraperIsNull()
    {
        // Act
        var ex = Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new SemaphoreScrapeRunner(null!, _loggerMock.Object, _progressMock.Object);
        });

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("scraper"));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenLoggerIsNull()
    {
        // Act
        var ex = Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new SemaphoreScrapeRunner(_scraperMock.Object, null!, _progressMock.Object);
        });

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("logger"));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenProgressBarManagerIsNull()
    {
        // Act
        var ex = Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new SemaphoreScrapeRunner(_scraperMock.Object, _loggerMock.Object, null!);
        });

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("progressBarManager"));
    }
}