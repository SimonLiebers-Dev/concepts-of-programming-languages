using WebScraper.Cli.Configuration;

namespace WebScraper.Cli.Tests.Configuration;

[TestFixture]
public class ScrapeConfigTests
{
    [Test]
    public void Validate_ShouldThrow_WhenUrlsFileIsMissing()
    {
        // Arrange
        var config = new ScrapeConfig
        {
            UrlsFile = "",
            ResultsDirectory = "results",
            Concurrency = 5,
            HttpTimeoutSeconds = 10
        };

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => config.Validate());

        // Assert
        Assert.That(ex!.Message, Does.Contain("UrlsFile"));
    }

    [Test]
    public void Validate_ShouldThrow_WhenResultsDirectoryIsMissing()
    {
        // Arrange
        var config = new ScrapeConfig
        {
            UrlsFile = "urls.json",
            ResultsDirectory = "",
            Concurrency = 5,
            HttpTimeoutSeconds = 10
        };

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => config.Validate());

        // Assert
        Assert.That(ex!.Message, Does.Contain("ResultsDirectory"));
    }

    [Test]
    public void Validate_ShouldThrow_WhenConcurrencyIsZeroOrNegative()
    {
        // Arrange
        var config = new ScrapeConfig
        {
            UrlsFile = "urls.json",
            ResultsDirectory = "results",
            Concurrency = 0,
            HttpTimeoutSeconds = 10
        };

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => config.Validate());

        // Assert
        Assert.That(ex!.Message, Does.Contain("Concurrency"));
    }

    [Test]
    public void Validate_ShouldThrow_WhenHttpTimeoutSecondsIsZeroOrNegative()
    {
        // Arrange
        var config = new ScrapeConfig
        {
            UrlsFile = "urls.json",
            ResultsDirectory = "results",
            Concurrency = 3,
            HttpTimeoutSeconds = 0
        };

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => config.Validate());

        // Assert
        Assert.That(ex!.Message, Does.Contain("HttpTimeoutSeconds"));
    }

    [Test]
    public void Validate_ShouldNotThrow_WhenAllValuesAreValid()
    {
        // Arrange
        var config = new ScrapeConfig
        {
            UrlsFile = "urls.json",
            ResultsDirectory = "results",
            Concurrency = 4,
            HttpTimeoutSeconds = 15
        };

        // Act + Assert
        Assert.DoesNotThrow(() => config.Validate());
    }

    [TestCase("   ")]
    [TestCase(null)]
    public void Validate_ShouldThrow_WhenUrlsFileIsWhitespaceOrNull(string? value)
    {
        // Arrange
        var config = new ScrapeConfig
        {
            UrlsFile = value ?? "",
            ResultsDirectory = "results",
            Concurrency = 2,
            HttpTimeoutSeconds = 5
        };

        // Act + Assert
        var ex = Assert.Throws<InvalidOperationException>(() => config.Validate());
        Assert.That(ex!.Message, Does.Contain("UrlsFile"));
    }

    [TestCase("   ")]
    [TestCase(null)]
    public void Validate_ShouldThrow_WhenResultsDirectoryIsWhitespaceOrNull(string? value)
    {
        // Arrange
        var config = new ScrapeConfig
        {
            UrlsFile = "urls.json",
            ResultsDirectory = value ?? "",
            Concurrency = 2,
            HttpTimeoutSeconds = 5
        };

        // Act + Assert
        var ex = Assert.Throws<InvalidOperationException>(() => config.Validate());
        Assert.That(ex!.Message, Does.Contain("ResultsDirectory"));
    }
}