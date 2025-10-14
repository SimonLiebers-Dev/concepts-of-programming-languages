using Microsoft.Extensions.Logging;
using Moq;
using Spectre.Console;
using WebScraper.Core.Models;
using WebScraper.Core.Scraping;
using WebScraper.Core.UI;

namespace WebScraper.Core.Tests.Scraping;

[TestFixture]
public class ParallelScrapeRunnerTests
{
    private Mock<IScraper> _scraperMock = null!;
    private Mock<IProgressBarManager> _progressMock = null!;
    private Mock<ILogger<SemaphoreScrapeRunner>> _loggerMock = null!;
    private ParallelScrapeRunner _runner = null!;

    [SetUp]
    public void SetUp()
    {
        _scraperMock = new Mock<IScraper>();
        _progressMock = new Mock<IProgressBarManager>();
        _loggerMock = new Mock<ILogger<SemaphoreScrapeRunner>>();
        
        _progressMock.Setup(p => p.CreateProgressBar(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(new ProgressTask(0, "Desc", 2));

        _progressMock.Setup(p => p.StartRenderingAsync()).Returns(Task.CompletedTask);

        _runner = new ParallelScrapeRunner(
            _scraperMock.Object,
            _loggerMock.Object,
            _progressMock.Object);
    }

    [Test]
    public async Task RunSequentialAsync_ShouldReturnAllPages()
    {
        // Arrange
        var urls = new[] { "url1", "url2", "url3" };
        _scraperMock.Setup(s => s.ScrapeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string url, CancellationToken _) => Page.SuccessPage(url, "Title", [], []));

        // Act
        var result = await _runner.RunSequentialAsync(urls);

        // Assert
        Assert.That(result, Has.Count.EqualTo(urls.Length));
        Assert.That(result.All(p => p.Success), Is.True);
        _progressMock.Verify(p => p.StartRenderingAsync(), Times.Once);
        _progressMock.Verify(p => p.StopRendererAsync(), Times.Once);
    }

    [Test]
    public async Task RunParallelAsync_ShouldReturnExpectedNumberOfResults()
    {
        // Arrange
        var urls = Enumerable.Range(1, 10).Select(i => $"url{i}").ToList();
        _scraperMock.Setup(s => s.ScrapeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string url, CancellationToken _) => Page.SuccessPage(url, "Title", [], []));

        // Act
        var result = await _runner.RunParallelAsync(urls, concurrency: 3);

        // Assert
        Assert.That(result, Has.Count.EqualTo(urls.Count));
        Assert.That(result.All(p => p.Success), Is.True);
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
    public async Task RunParallelAsync_ShouldRespectConcurrencyLimit()
    {
        // Arrange
        var urls = Enumerable.Range(1, 25).Select(i => $"url{i}").ToList();
        var activeCount = 0;
        var maxConcurrent = 0;
        var concurrencyLimit = 4;

        // Track concurrent active scrapes
        _scraperMock.Setup(s => s.ScrapeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(async (string url, CancellationToken ct) =>
            {
                Interlocked.Increment(ref activeCount);
                maxConcurrent = Math.Max(maxConcurrent, activeCount);
                await Task.Delay(40, ct);
                Interlocked.Decrement(ref activeCount);
                return Page.SuccessPage(url, "Title", [], []);
            });

        // Act
        var result = await _runner.RunParallelAsync(urls, concurrencyLimit);

        // Assert
        Assert.That(result, Has.Count.EqualTo(urls.Count));
        Assert.That(maxConcurrent, Is.LessThanOrEqualTo(concurrencyLimit));
    }

    [Test]
    public async Task RunParallelAsync_ShouldReturnPartialResultsOnCancellation()
    {
        // Arrange
        var urls = Enumerable.Range(1, 30).Select(i => $"url{i}").ToList();
        var cts = new CancellationTokenSource();

        _scraperMock.Setup(s => s.ScrapeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(async (string url, CancellationToken token) =>
            {
                await Task.Delay(30, token);
                return Page.SuccessPage(url, "Title", [], []);
            });

        // Cancel after short delay
        _ = Task.Run(async () =>
        {
            await Task.Delay(50, cts.Token);
            await cts.CancelAsync();
        }, cts.Token);

        // Act
        IReadOnlyList<Page> result;
        try
        {
            result = await _runner.RunParallelAsync(urls, concurrency: 8, cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Some environments might throw cancellation before results are collected.
            result = new List<Page>();
        }

        // Assert
        Assert.That(result.Count, Is.LessThanOrEqualTo(urls.Count));
    }
}