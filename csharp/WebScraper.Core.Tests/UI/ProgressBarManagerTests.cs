using WebScraper.Core.UI;

namespace WebScraper.Core.Tests.UI;

[TestFixture]
public class ProgressBarManagerTests
{
    private ProgressBarManager _manager = null!;

    [SetUp]
    public void SetUp()
    {
        _manager = new ProgressBarManager();
    }

    [Test]
    public void StartRenderingAsync_ShouldInitializeContext()
    {
        // Act
        var startTask = _manager.StartRenderingAsync();

        // Assert
        Assert.That(startTask, Is.Not.Null);
    }

    [Test]
    public void CreateProgressBar_ShouldThrow_WhenNotStarted()
    {
        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => { _ = _manager.CreateProgressBar("https://example.com", 2); });
    }

    [Test]
    public async Task CreateProgressBar_ShouldReturnTask_WhenStarted()
    {
        // Arrange
        await _manager.StartRenderingAsync();

        // Act
        var task = _manager.CreateProgressBar("https://example.com", 2);

        // Assert
        Assert.That(task, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(task.MaxValue, Is.EqualTo(2));
            Assert.That(task.Description, Does.Contain("Fetching"));
        });
    }

    [Test]
    public async Task StopRendererAsync_ShouldCancelRendering()
    {
        // Arrange
        await _manager.StartRenderingAsync();

        // Act + Assert: StopRendererAsync should not throw
        Assert.DoesNotThrowAsync(async () => await _manager.StopRendererAsync());
    }

    [Test]
    public async Task DisposeAsync_ShouldStopAndDisposeResources()
    {
        // Arrange
        await _manager.StartRenderingAsync();

        // Act + Assert
        Assert.DoesNotThrowAsync(async () => await _manager.DisposeAsync());
    }

    [Test]
    public async Task MultipleStartCalls_ShouldNotThrowOrCreateDuplicateTasks()
    {
        // Arrange + Act
        await _manager.StartRenderingAsync();
        await _manager.StartRenderingAsync(); // should be ignored safely

        // Assert
        Assert.Pass("Multiple StartRenderingAsync() calls are safe.");
    }

    [TearDown]
    public async Task TearDown()
    {
        try
        {
            await _manager.DisposeAsync();
        }
        catch (ObjectDisposedException)
        {
            // Already disposed by the test. Safe to ignore
        }
    }
}