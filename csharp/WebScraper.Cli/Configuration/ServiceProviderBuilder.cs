using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebScraper.Cli.App;
using WebScraper.Core.DependencyInjection;

namespace WebScraper.Cli.Configuration;

/// <summary>
/// Provides a factory for constructing a configured <see cref="IServiceProvider"/> instance
/// used to resolve application dependencies.
/// </summary>
/// <remarks>
/// This class sets up dependency injection and logging
/// for the web scraper CLI application. It can be used as the single
/// entry point for creating the application's service provider.
/// </remarks>
public static class ServiceProviderBuilder
{
    /// <summary>
    /// Creates and configures a new <see cref="IServiceProvider"/> instance.
    /// </summary>
    /// <remarks>
    /// The returned service provider includes:
    /// <list type="bullet">
    /// <item><description>Console logging via <see cref="Microsoft.Extensions.Logging"/></description></item>
    /// <item><description>Core scraper services registered through <c>AddWebScraperCore</c></description></item>
    /// <item><description>The main <see cref="Application"/> class registered as a transient</description></item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// A fully configured <see cref="IServiceProvider"/> that can be used to resolve dependencies
    /// such as <see cref="Application"/>, logging, or core web scraper services.
    /// </returns>
    public static IServiceProvider CreateServiceProvider(bool enableLogging = false)
    {
        // Set up DI container
        ServiceCollection services = new();

        // Register logging provider if enableLogging is true
        if (enableLogging)
        {
            services.AddLogging(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "HH:mm:ss ";
                    options.SingleLine = true;
                });
            });
        }

        // Add web scraper core with configured httpClient timeout
        services.AddWebScraperCore(configureHttpClient: client => { client.Timeout = TimeSpan.FromSeconds(10); });

        // Register main application class
        services.AddTransient<Application>();

        // Build and return the provider
        return services.BuildServiceProvider();
    }
}