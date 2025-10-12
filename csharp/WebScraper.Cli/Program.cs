using Microsoft.Extensions.DependencyInjection;
using WebScraper.Cli.App;
using WebScraper.Cli.Configuration;

namespace WebScraper.Cli;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var provider = ServiceProviderBuilder.CreateServiceProvider();
        
        // Run the app
        var app = provider.GetRequiredService<Application>();
        await app.RunAsync();
    }
}