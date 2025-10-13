using Microsoft.Extensions.DependencyInjection;
using WebScraper.Cli.App;
using WebScraper.Cli.Configuration;

namespace WebScraper.Cli;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        // Create service provider
        var provider = ServiceProviderBuilder.CreateServiceProvider(enableLogging: false);

        // Run
        await RunAsync(args, provider);
    }

    /// <summary>
    /// Testable entry point for running the CLI application.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <param name="provider">
    /// Optional service provider for dependency injection.  
    /// If <see langword="null"/>, a new provider is created via <see cref="ServiceProviderBuilder"/>.
    /// </param>
    internal static async Task RunAsync(string[] args, IServiceProvider? provider = null)
    {
        provider ??= ServiceProviderBuilder.CreateServiceProvider(enableLogging: false);

        var app = provider.GetRequiredService<IApplication>();
        await app.RunAsync();
    }
}