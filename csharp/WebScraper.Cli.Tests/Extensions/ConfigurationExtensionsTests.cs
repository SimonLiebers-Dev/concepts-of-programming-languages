using Microsoft.Extensions.Configuration;
using WebScraper.Cli.Extensions;

namespace WebScraper.Cli.Tests.Extensions;

[TestFixture]
public class ConfigurationExtensionsTests
{
    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    [Test]
    public void TryGetScrapeConfig_ShouldReturnTrue_WhenConfigIsValid()
    {
        // Arrange
        var values = new Dictionary<string, string?>
        {
            ["Scraper:UrlsFile"] = "urls.json",
            ["Scraper:ResultsDirectory"] = "results",
            ["Scraper:Concurrency"] = "4",
            ["Scraper:HttpTimeoutSeconds"] = "15"
        };
        var configuration = BuildConfiguration(values);

        // Act
        var result = configuration.TryGetScrapeConfig(out var config, out var error);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(error, Is.Null);
            Assert.That(config.UrlsFile, Is.EqualTo("urls.json"));
            Assert.That(config.ResultsDirectory, Is.EqualTo("results"));
            Assert.That(config.Concurrency, Is.EqualTo(4));
            Assert.That(config.HttpTimeoutSeconds, Is.EqualTo(15));
        });
    }

    [Test]
    public void TryGetScrapeConfig_ShouldReturnFalse_WhenSectionMissing()
    {
        // Arrange
        var configuration = BuildConfiguration(new Dictionary<string, string?>());

        // Act
        var result = configuration.TryGetScrapeConfig(out var config, out var error);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(error, Does.Contain("Missing required configuration section"));
        Assert.That(config, Is.Not.Null);
    }

    [Test]
    public void TryGetScrapeConfig_ShouldReturnFalse_WhenBindingFails()
    {
        // Arrange
        // Missing values cause the bound object to be null
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["OtherSection:Key"] = "value"
        });

        // Act
        var result = configuration.TryGetScrapeConfig(out var config, out var error);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(error, Does.Contain("Missing required configuration section"));
        });
    }

    [Test]
    public void TryGetScrapeConfig_ShouldReturnFalse_WhenValidationFails()
    {
        // Arrange
        var values = new Dictionary<string, string?>
        {
            ["Scraper:UrlsFile"] = "", // Invalid
            ["Scraper:ResultsDirectory"] = "results",
            ["Scraper:Concurrency"] = "0", // Invalid
            ["Scraper:HttpTimeoutSeconds"] = "0" // Invalid
        };
        var configuration = BuildConfiguration(values);

        // Act
        var result = configuration.TryGetScrapeConfig(out var config, out var error);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(error, Does.Contain("Configuration validation failed"));
        });
        Assert.Multiple(() =>
        {
            Assert.That(error, Does.Contain("UrlsFile").Or.Contain("Concurrency").Or.Contain("HttpTimeoutSeconds"));
            Assert.That(config, Is.Not.Null);
        });
    }

    [Test]
    public void TryGetScrapeConfig_ShouldThrow_WhenConfigurationIsNull()
    {
        // Arrange
        IConfiguration? configuration = null;

        // Act + Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            configuration!.TryGetScrapeConfig(out _, out _));

        Assert.That(ex!.ParamName, Is.EqualTo("configuration"));
    }

    [Test]
    public void TryGetScrapeConfig_ShouldReturnFalse_WhenSectionExistsButValuesInvalid()
    {
        // Arrange
        var values = new Dictionary<string, string?>
        {
            ["Scraper:UrlsFile"] = "urls.json",
            ["Scraper:ResultsDirectory"] = "", // Missing
            ["Scraper:Concurrency"] = "-1", // Invalid
            ["Scraper:HttpTimeoutSeconds"] = "5"
        };
        var configuration = BuildConfiguration(values);

        // Act
        var result = configuration.TryGetScrapeConfig(out var config, out var error);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(error, Does.Contain("Configuration validation failed"));
            Assert.That(config, Is.Not.Null);
        });
    }
}