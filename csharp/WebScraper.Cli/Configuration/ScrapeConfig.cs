namespace WebScraper.Cli.Configuration;

/// <summary>
/// Represents the configuration settings required for the web scraper.
/// </summary>
internal class ScrapeConfig
{
    /// <summary>
    /// Gets the path to the JSON file containing the list of URLs to scrape.
    /// </summary>
    public string UrlsFile { get; init; } = string.Empty;

    /// <summary>
    /// Gets the directory where scrape results will be saved.
    /// </summary>
    public string ResultsDirectory { get; init; } = string.Empty;

    /// <summary>
    /// Gets the number of concurrent workers used during parallel scraping.
    /// </summary>
    public int Concurrency { get; init; }

    /// <summary>
    /// Gets the HTTP timeout (in seconds) used by the fetcher.
    /// </summary>
    public int HttpTimeoutSeconds { get; init; }

    /// <summary>
    /// Validates that all configuration values are set and valid.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any required configuration value is missing or invalid.
    /// </exception>
    public void Validate()
    {
        // Throw exception if url file is not configured
        if (string.IsNullOrWhiteSpace(UrlsFile))
            throw new InvalidOperationException("Missing required configuration: Scraper:UrlsFile");

        // Throw exception if result directory is not configured
        if (string.IsNullOrWhiteSpace(ResultsDirectory))
            throw new InvalidOperationException("Missing required configuration: Scraper:ResultsDirectory");

        // Throw exception if concurrency is not configured and not bigger than 0
        if (Concurrency <= 0)
            throw new InvalidOperationException("Scraper:Concurrency must be greater than zero.");

        // Throw exception if http timeout is not configured and not bigger than 0
        if (HttpTimeoutSeconds <= 0)
            throw new InvalidOperationException("Scraper:HttpTimeoutSeconds must be greater than zero.");
    }
}