using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebScraper.Cli.App;
using WebScraper.Cli.Util;
using WebScraper.Core.Fetcher;
using WebScraper.Core.Parser;
using WebScraper.Core.Scraping;
using WebScraper.Core.UI;

namespace WebScraper.Cli.Tests.Util;

[TestFixture]
public class ServiceProviderUtilsTests
{
    [Test]
    public void CreateServiceProvider_ShouldReturnValidProvider()
    {
        // Act
        var provider = ServiceProviderUtils.CreateServiceProvider();

        // Assert
        Assert.That(provider, Is.Not.Null, "Provider should not be null");

        var config = provider.GetService<IConfiguration>();
        Assert.Multiple(() =>
        {
            // Verify configuration is registered
            Assert.That(config, Is.Not.Null, "IConfiguration should be registered");

            // Verify scraper core services
            Assert.That(provider.GetService<IHtmlFetcher>(), Is.Not.Null);
            Assert.That(provider.GetService<IHtmlParser>(), Is.Not.Null);
            Assert.That(provider.GetService<IScraper>(), Is.Not.Null);
            Assert.That(provider.GetService<IProgressBarManager>(), Is.Not.Null);
            Assert.That(provider.GetService<IScrapeRunner>(), Is.Not.Null);

            // Verify main Application is registered
            Assert.That(provider.GetService<IApplication>(), Is.Not.Null);
        });
    }

    [Test]
    public void CreateServiceProvider_ShouldResolveAllDependenciesForApplication()
    {
        // Arrange
        var provider = ServiceProviderUtils.CreateServiceProvider();

        // Act + Assert
        var app = provider.GetRequiredService<IApplication>();
        Assert.That(app, Is.Not.Null, "Application should be resolvable from DI");
    }

    [Test]
    public void CreateServiceProvider_ShouldIncludeLogging_WhenEnabled()
    {
        // Act
        var provider = ServiceProviderUtils.CreateServiceProvider(enableLogging: true);

        // Assert
        var loggerFactory = provider.GetService<ILoggerFactory>();
        Assert.That(loggerFactory, Is.Not.Null, "ILoggerFactory should be registered when logging is enabled");

        var logger = loggerFactory!.CreateLogger<ServiceProviderUtilsTests>();
        Assert.DoesNotThrow(() => logger.LogInformation("Logging works!"));
    }

    [Test]
    public void CreateServiceProvider_ShouldBuildProviderWithoutThrowing()
    {
        // Act + Assert
        Assert.DoesNotThrow(() => ServiceProviderUtils.CreateServiceProvider());
    }

    [Test]
    public void CreateServiceProvider_ShouldReturnNewInstances()
    {
        // Arrange
        var provider = ServiceProviderUtils.CreateServiceProvider();

        // Act
        var scraper1 = provider.GetRequiredService<IScraper>();
        var scraper2 = provider.GetRequiredService<IScraper>();

        // Assert
        Assert.That(scraper1, Is.Not.SameAs(scraper2), "IScraper should be transient.");
    }
}