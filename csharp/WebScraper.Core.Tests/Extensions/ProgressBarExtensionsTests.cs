using Spectre.Console;
using WebScraper.Core.Extensions;

namespace WebScraper.Core.Tests.Extensions;

[TestFixture]
public class ProgressBarExtensionsTests
{
    [Test]
    public void MarkAsDone_ShouldSetTaskToMaxValue_AndUpdateDescription()
    {
        // Arrange
        const string url = "Test Url";
        var task = new ProgressTask(0, url, maxValue: 2);

        // Act
        task.MarkAsDone(url);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.Value, Is.EqualTo(task.MaxValue));
            Assert.That(task.IsFinished, Is.True);
            Assert.That(task.Description, Contains.Substring("Success").And.Contains("green"));
        });
    }

    [Test]
    public void MarkAsError_ShouldSetTaskToMaxValue_AndUpdateDescription()
    {
        // Arrange
        const string url = "Test Url";
        var task = new ProgressTask(0, url, maxValue: 2);

        // Act
        task.MarkAsError(url);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(task.Value, Is.EqualTo(0));
            Assert.That(task.IsFinished, Is.True);
            Assert.That(task.Description, Contains.Substring("Error").And.Contains("red"));
        });
    }

    [Test]
    public void MarkAsDone_ShouldNotThrow_WhenAlreadyFinished()
    {
        // Arrange
        const string url = "Test Url";
        var task = new ProgressTask(0, url, maxValue: 2);
        task.Value = task.MaxValue;
        task.StopTask();

        // Act + Assert
        Assert.DoesNotThrow(() => task.MarkAsDone(url));
    }

    [Test]
    public void MarkAsError_ShouldNotThrow_WhenAlreadyFinished()
    {
        // Arrange
        const string url = "Test Url";
        var task = new ProgressTask(0, url, maxValue: 2);
        task.Value = task.MaxValue;
        task.StopTask();

        // Act + Assert
        Assert.DoesNotThrow(() => task.MarkAsError(url));
    }
}