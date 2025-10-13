using Microsoft.Extensions.DependencyInjection;
using WebScraper.Core.DependencyInjection;
using WebScraper.Core.Fetcher;
using WebScraper.Core.Parser;
using WebScraper.Core.Scraping;
using WebScraper.Core.UI;

namespace WebScraper.Core.Tests.DependencyInjection;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    private ServiceCollection _services = null!;

    [SetUp]
    public void SetUp()
    {
        _services = [];
    }

    [Test]
    public void AddWebScraperCore_ShouldRegisterAllRequiredServices()
    {
        // Act
        _services.AddWebScraperCore();
        var provider = _services.BuildServiceProvider();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(provider.GetService<IHtmlFetcher>(), Is.Not.Null, "IHtmlFetcher should be registered.");
            Assert.That(provider.GetService<IHtmlParser>(), Is.Not.Null, "IHtmlParser should be registered.");
            Assert.That(provider.GetService<IScraper>(), Is.Not.Null, "IScraper should be registered.");
            Assert.That(provider.GetService<IProgressBarManager>(), Is.Not.Null,
                "IProgressBarManager should be registered.");
            Assert.That(provider.GetService<IScrapeRunner>(), Is.Not.Null, "IScrapeRunner should be registered.");
        });
    }

    [Test]
    public void AddWebScraperCore_ShouldReturnSameServiceCollectionInstance()
    {
        // Act
        var result = _services.AddWebScraperCore();

        // Assert
        Assert.That(result, Is.SameAs(_services), "Should return same IServiceCollection for chaining.");
    }

    [Test]
    public void AddWebScraperCore_ShouldConfigureHttpClient_WhenDelegateProvided()
    {
        // Arrange
        var timeout = TimeSpan.FromSeconds(42);
        _services.AddWebScraperCore(client => client.Timeout = timeout);
        var provider = _services.BuildServiceProvider();

        // Act
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient(nameof(IHtmlFetcher));

        // Assert
        Assert.That(client.Timeout, Is.EqualTo(timeout),
            "HttpClient should respect timeout configured via delegate.");
    }

    [Test]
    public void AddWebScraperCore_ShouldRegisterTransientLifetimes()
    {
        // Act
        _services.AddWebScraperCore();
        var provider = _services.BuildServiceProvider();

        // Act + Assert
        var scraper1 = provider.GetRequiredService<IScraper>();
        var scraper2 = provider.GetRequiredService<IScraper>();
        Assert.That(scraper1, Is.Not.SameAs(scraper2), "IScraper should be transient.");

        var runner1 = provider.GetRequiredService<IScrapeRunner>();
        var runner2 = provider.GetRequiredService<IScrapeRunner>();
        Assert.That(runner1, Is.Not.SameAs(runner2), "IScrapeRunner should be transient.");

        var parser1 = provider.GetRequiredService<IHtmlParser>();
        var parser2 = provider.GetRequiredService<IHtmlParser>();
        Assert.That(parser1, Is.Not.SameAs(parser2), "IHtmlParser should be transient.");
    }
}