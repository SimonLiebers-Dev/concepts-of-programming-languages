using System.Text.Json;
using WebScraper.Core.Models;

namespace WebScraper.Cli.Util;

/// <summary>
/// Provides helper methods for reading configuration files and saving scraper results.
/// </summary>
/// <remarks>
/// This class uses the built-in <see cref="System.IO"/> APIs and is designed to be easily testable
/// without requiring custom abstractions. For deterministic tests, callers can mock
/// <see cref="DateTimeOffset.UtcNow"/> or inject custom time values.
/// </remarks>
public static class FileUtils
{
    /// <summary>
    /// Json serializer options used across methods
    /// </summary>
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Reads URLs from a JSON configuration file or creates an empty one if it does not exist.
    /// </summary>
    /// <param name="configFile">The path to the configuration file, e.g. <c>"urls.json"</c>.</param>
    /// <returns>
    /// A list of URLs read from the file.  
    /// If the file does not exist, it will be created with an empty JSON array (<c>[]</c>).
    /// </returns>
    /// <exception cref="IOException">Thrown when the file cannot be read or written.</exception>
    /// <exception cref="JsonException">Thrown when the file contains invalid JSON.</exception>
    public static async Task<List<string>> GetUrlsFromFileAsync(string configFile)
    {
        // Create the file if it doesn't exist
        if (!File.Exists(configFile))
        {
            var json = JsonSerializer.Serialize(new List<string>(), JsonSerializerOptions);
            await File.WriteAllTextAsync(configFile, json).ConfigureAwait(false);
            return [];
        }

        try
        {
            var json = await File.ReadAllTextAsync(configFile).ConfigureAwait(false);
            var urls = JsonSerializer.Deserialize<List<string>>(json);

            return urls ?? [];
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Invalid JSON in {configFile}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new IOException($"Could not read {configFile}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Saves the given pages to a timestamped JSON file in the specified output directory.
    /// </summary>
    /// <param name="pages">The list of scraped pages to save.</param>
    /// <param name="outputDirectory">
    /// The directory where the file should be saved.  
    /// If the directory does not exist, it will be created automatically.
    /// </param>
    /// <param name="timestamp">
    /// Optional timestamp used for deterministic output (useful for testing).  
    /// If <see langword="null"/>, <see cref="DateTimeOffset.UtcNow"/> will be used.
    /// </param>
    /// <returns>
    /// The full path to the file that was created, e.g. <c>results/scrape-results-1234567890.json</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="pages"/> is empty.</exception>
    /// <exception cref="IOException">Thrown when the file cannot be written.</exception>
    public static async Task<string> SaveResultsToFileAsync(
        IEnumerable<Page> pages,
        string outputDirectory,
        DateTimeOffset timestamp)
    {
        ArgumentException.ThrowIfNullOrEmpty(outputDirectory);
        var pageList = pages.ToList();

        if (pageList.Count == 0)
            throw new ArgumentException("No pages to save.", nameof(pages));

        // Ensure directory exists
        Directory.CreateDirectory(outputDirectory);

        var millis = timestamp.ToUnixTimeMilliseconds();
        var filename = $"scrape-results-{millis}.json";
        var filePath = Path.Combine(outputDirectory, filename);

        try
        {
            var json = JsonSerializer.Serialize(pageList, JsonSerializerOptions);
            await File.WriteAllTextAsync(filePath, json).ConfigureAwait(false);
            return filePath;
        }
        catch (Exception ex)
        {
            throw new IOException($"Could not write file {filePath}: {ex.Message}", ex);
        }
    }
}