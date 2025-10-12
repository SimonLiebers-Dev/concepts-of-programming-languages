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
    /// <param name="configureHttpClient">
    /// Optional delegate to configure the <see cref="HttpClient"/> used by the <see cref="IHtmlFetcher"/>.
    /// </param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddWebScraperCore(
        this IServiceCollection services,
        Action<HttpClient>? configureHttpClient = null)
    {
        // Add HTTP fetcher with optional custom configuration
        var builder = services.AddHttpClient<IHtmlFetcher, HtmlFetcher>();
        
        if (configureHttpClient is not null)
            builder.ConfigureHttpClient(configureHttpClient);

        // Add HTML parser
        services.AddTransient<IHtmlParser, HtmlParser>();
        
        // Add HTML fetcher
        services.AddTransient<IHtmlFetcher, HtmlFetcher>();
        
        // Add web scraper
        services.AddTransient<IScraper, DefaultWebScraper>();
        
        // Add web scraper
        services.AddSingleton<IProgressBarManager, ProgressBarManager>();
        
        // Add scrape runner
        services.AddSingleton<IScrapeRunner, DefaultScrapeRunner>();

        return services;
    }
}