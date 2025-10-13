using Microsoft.Extensions.Configuration;
using WebScraper.Cli.Configuration;

namespace WebScraper.Cli.Extensions;

/// <summary>
/// Provides extension methods for reading and validating scraper configuration.
/// </summary>
internal static class ConfigurationExtensions
{
    private const string ScraperSection = "Scraper";

    /// <summary>
    /// Attempts to read and validate the <see cref="ScrapeConfig"/> section from configuration.
    /// </summary>
    /// <param name="configuration">The configuration source.</param>
    /// <param name="config">
    /// When this method returns, contains the populated and validated <see cref="ScrapeConfig"/> instance
    /// if the operation succeeded; otherwise an object with unset parameters.
    /// </param>
    /// <param name="error">
    /// When this method returns <see langword="false"/>, contains a human-readable error message describing
    /// why validation failed.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the configuration section exists and is valid; otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryGetScrapeConfig(
        this IConfiguration configuration,
        out ScrapeConfig config,
        out string? error)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        config = new ScrapeConfig();
        error = null;

        var section = configuration.GetSection(ScraperSection);
        if (!section.Exists())
        {
            error = $"Missing required configuration section: '{ScraperSection}'";
            return false;
        }

        var bound = section.Get<ScrapeConfig>();
        if (bound is null)
        {
            error = "Failed to bind Scraper configuration section.";
            return false;
        }

        try
        {
            bound.Validate();
            config = bound;
            return true;
        }
        catch (Exception ex)
        {
            error = $"Configuration validation failed: {ex.Message}";
            return false;
        }
    }
}