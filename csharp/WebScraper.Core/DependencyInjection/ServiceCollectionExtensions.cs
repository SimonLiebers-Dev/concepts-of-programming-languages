using Microsoft.Extensions.DependencyInjection;
using WebScraper.Core.Fetcher;
using WebScraper.Core.Parser;
using WebScraper.Core.Scraping;
using WebScraper.Core.UI;

namespace WebScraper.Core.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the WebScraper Core services
/// into a dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all core WebScraper services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The DI service collection to register dependencies into.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddWebScraperCore<TRunner>(
        this IServiceCollection services)
        where TRunner : class, IScrapeRunner
    {
        // Add HTTP fetcher
        services.AddHttpClient<IHtmlFetcher, HtmlFetcher>();

        // Add HTML parser
        services.AddTransient<IHtmlParser, HtmlParser>();

        // Add HTML fetcher
        services.AddTransient<IHtmlFetcher, HtmlFetcher>();

        // Add web scraper
        services.AddTransient<IScraper, DefaultWebScraper>();

        // Add web scraper
        services.AddTransient<IProgressBarManager, ProgressBarManager>();

        // Add scrape runner
        return services.AddTransient<IScrapeRunner, TRunner>();

        return services;
    }
}