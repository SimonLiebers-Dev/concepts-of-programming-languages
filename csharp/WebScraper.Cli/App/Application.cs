using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using WebScraper.Cli.Configuration;
using WebScraper.Cli.Extensions;
using WebScraper.Core.Fetcher;
using WebScraper.Core.Models;
using WebScraper.Core.Scraping;
using WebScraper.Core.Util;

namespace WebScraper.Cli.App;

/// <inheritdoc cref="IApplication"/>
internal class Application : IApplication
{
    private readonly IConfiguration _configuration;
    private readonly IHtmlFetcher _fetcher;
    private readonly IScrapeRunner _runner;

    /// <summary>
    /// Initializes a new instance of the <see cref="Application"/> class.
    /// </summary>
    /// <param name="configuration">The configuration provider for application settings.</param>
    /// <param name="runner">The runner that performs sequential or parallel scraping operations.</param>
    /// <param name="fetcher">The fetcher that performs requests to target URLs.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any dependency is <see langword="null"/>.
    /// </exception>
    public Application(IConfiguration configuration, IHtmlFetcher fetcher, IScrapeRunner runner)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _fetcher = fetcher ?? throw new ArgumentNullException(nameof(fetcher));
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
    }

    /// <inheritdoc cref="IApplication"/>
    public async Task RunAsync(CancellationToken ct = default)
    {
        LayoutUtils.PrintHeader();
        LayoutUtils.PrintSeparator();

        // Try read config from configuration
        if (!_configuration.TryGetScrapeConfig(out var config, out var error))
        {
            Console.WriteLine(error);
            return;
        }

        // Apply config to fetcher
        _fetcher.SetHttpTimeout(config.HttpTimeoutSeconds);
        _fetcher.SetUserAgent(config.UserAgent);

        // Read URLs or create empty file
        List<string> urls;
        try
        {
            urls = await FileUtils.GetUrlsFromFileAsync(config.UrlsFile).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"URLs could not be loaded from {config.UrlsFile}. Please check your json file.");
            return;
        }

        // Print config
        PrintConfig(config, urls.Count);

        // Print warning that no urls have been configured
        if (urls.Count == 0)
        {
            Console.WriteLine("⚠️  No URLs configured.");
            Console.WriteLine($"📄  Please add URLs to '{config.UrlsFile}' before running the scraper.");
            return;
        }

        LayoutUtils.PrintSeparator();

        var choice = PromptMode();
        LayoutUtils.PrintSeparator();

        var stopwatch = Stopwatch.StartNew();
        var results = choice == 1
            ? await RunSequentialAsync(urls, ct)
            : await RunParallelAsync(urls, config, ct);
        stopwatch.Stop();

        PrintSummary(results, stopwatch.Elapsed);
        LayoutUtils.PrintSeparator();

        await PromptSaveResultsAsync(results, config).ConfigureAwait(false);
    }

    private static int PromptMode()
    {
        while (true)
        {
            Console.WriteLine("Choose scraping mode:");
            Console.WriteLine("1 - Sequential");
            Console.WriteLine("2 - Parallel");
            LayoutUtils.PrintSeparator();

            var input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out var parsed) && parsed is 1 or 2)
                return parsed;

            LayoutUtils.PrintSeparator();
            Console.WriteLine("❌ Invalid input. Please enter 1 or 2.");
            LayoutUtils.PrintSeparator();
        }
    }

    private static void PrintConfig(ScrapeConfig config, int urlCount)
    {
        Console.WriteLine($"📄  URLs File: {config.UrlsFile} ({urlCount} urls loaded)");
        Console.WriteLine($"💾  Results Directory: {config.ResultsDirectory}/");
        Console.WriteLine($"⚙️  Concurrency: {config.Concurrency}");
        Console.WriteLine($"⏱️  HTTP Timeout (s): {config.HttpTimeoutSeconds}");

        var userAgent = config.UserAgent;
        if (userAgent.Length > 80)
        {
            userAgent = $"{userAgent[..80]}...";
        }

        Console.WriteLine($"🕸️  User-Agent: {userAgent}");
    }

    private async Task<IReadOnlyList<Page>> RunSequentialAsync(IReadOnlyList<string> urls, CancellationToken ct)
    {
        Console.WriteLine("🚀 Running sequential scraper...");
        LayoutUtils.PrintSeparator();
        return await _runner.RunSequentialAsync(urls, ct).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<Page>> RunParallelAsync(IReadOnlyList<string> urls, ScrapeConfig config,
        CancellationToken ct)
    {
        Console.WriteLine("🚀 Running parallel scraper...");
        LayoutUtils.PrintSeparator();
        return await _runner.RunParallelAsync(urls, config.Concurrency, ct).ConfigureAwait(false);
    }

    private static void PrintSummary(IReadOnlyList<Page> pages, TimeSpan duration)
    {
        var successCount = pages.Count(p => p.Success);
        var failedCount = pages.Count - successCount;
        Console.WriteLine($"✅ {successCount} successful | ❌ {failedCount} failed | ⏱️ Duration: {duration}");
    }

    private static async Task PromptSaveResultsAsync(IReadOnlyList<Page> pages, ScrapeConfig config)
    {
        while (true)
        {
            Console.Write("💾 Do you want to save the results to a file? (y/n): ");
            var input = Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (input)
            {
                case "y":
                    try
                    {
                        var filename = await FileUtils
                            .SaveResultsToFileAsync(pages, config.ResultsDirectory, DateTimeOffset.UtcNow)
                            .ConfigureAwait(false);
                        Console.WriteLine($"✅ Results saved to: {filename}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error saving file: {ex.Message}");
                    }

                    return;

                case "n":
                    Console.WriteLine("ℹ️  Results not saved.");
                    return;

                default:
                    Console.WriteLine("Please type 'y' or 'n'.");
                    break;
            }
        }
    }
}