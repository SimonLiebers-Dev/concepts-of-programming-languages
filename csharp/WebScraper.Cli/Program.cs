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
        
        // Get application service
        var app = provider.GetRequiredService<Application>();
        
        // Run application
        await app.RunAsync();
    }
}