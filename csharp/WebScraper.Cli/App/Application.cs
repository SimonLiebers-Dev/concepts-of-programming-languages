using System.Diagnostics;
using Microsoft.Extensions.Logging;
using WebScraper.Core.Models;
using WebScraper.Core.Scraping;
using WebScraper.Core.UI;
using WebScraper.Core.Util;

namespace WebScraper.Cli.App;

/// <summary>
/// Represents the top-level application entry point.
/// Handles user interaction, orchestrates scraping, prints summaries,
/// and optionally persists the results.
/// </summary>
public sealed class Application
{
    private readonly ILogger<Application> _logger;
    private readonly IScrapeRunner _runner;

    public Application(ILogger<Application> logger, IScrapeRunner runner)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
    }

    /// <summary>
    /// Executes the main interactive scraper workflow.
    /// </summary>
    /// <param name="ct">A cancellation token for graceful termination.</param>
    public async Task RunAsync(CancellationToken ct = default)
    {
        LayoutUtils.PrintHeader();
        LayoutUtils.PrintSeparator();

        const string urlsFile = "urls.json";

        // Read URLs or create empty file
        var urls = await FileUtils.GetUrlsFromFileAsync(urlsFile).ConfigureAwait(false);
        if (urls.Count == 0)
        {
            Console.WriteLine("⚠️  No URLs configured.");
            Console.WriteLine("📄  Please add URLs to 'urls.json' before running the scraper.");
            return;
        }

        Console.WriteLine($"Loaded {urls.Count} URLs from {urlsFile}");
        LayoutUtils.PrintSeparator();

        var choice = PromptMode();
        LayoutUtils.PrintSeparator();

        var stopwatch = Stopwatch.StartNew();
        var results = choice == 1
            ? await RunSequentialAsync(urls, ct)
            : await RunParallelAsync(urls, ct);
        stopwatch.Stop();

        PrintSummary(results, stopwatch.Elapsed);
        LayoutUtils.PrintSeparator();

        await PromptSaveResultsAsync(results).ConfigureAwait(false);
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
            if (int.TryParse(input, out var parsed) && (parsed == 1 || parsed == 2))
                return parsed;

            LayoutUtils.PrintSeparator();
            Console.WriteLine("❌ Invalid input. Please enter 1 or 2.");
            LayoutUtils.PrintSeparator();
        }
    }

    private async Task<IReadOnlyList<Page>> RunSequentialAsync(IReadOnlyList<string> urls, CancellationToken ct)
    {
        Console.WriteLine("🚀 Running sequential scraper...");
        LayoutUtils.PrintSeparator();
        return await _runner.RunSequentialAsync(urls, ct).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<Page>> RunParallelAsync(IReadOnlyList<string> urls, CancellationToken ct)
    {
        Console.WriteLine("🚀 Running parallel scraper...");
        LayoutUtils.PrintSeparator();
        return await _runner.RunParallelAsync(urls, concurrency: 5, ct).ConfigureAwait(false);
    }

    private static void PrintSummary(IReadOnlyList<Page> pages, TimeSpan duration)
    {
        var success = pages.Count(p => p.Success);
        var failed = pages.Count(p => p.HasError);
        Console.WriteLine($"✅ {success} successful | ❌ {failed} failed | ⏱️ Duration: {duration}");
    }

    private static async Task PromptSaveResultsAsync(IReadOnlyList<Page> pages)
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
                        var filename = await FileUtils.SaveResultsToFileAsync(pages).ConfigureAwait(false);
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