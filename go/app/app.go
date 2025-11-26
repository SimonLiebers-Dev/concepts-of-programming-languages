package app

import (
	"bufio"
	"context"
	"fmt"
	"go-scraper/config"
	"go-scraper/core"
	"go-scraper/models"
	"go-scraper/ui"
	"go-scraper/util"
	"os"
	"strconv"
	"time"
)

const (
	// configFile is the filename for application configuration
	configFile = "config.json"
	// userAgentTruncateLength is the maximum length for displaying user agent strings
	userAgentTruncateLength = 80
)

// Run is the top-level entry point for the scraper application.
// It orchestrates the entire scraping workflow:
//  1. Display header and load configuration
//  2. Load URLs from the configured file
//  3. Prompt user for scraping mode (sequential or parallel)
//  4. Execute scraping with progress tracking
//  5. Display summary results
//  6. Optionally save results to a file
//
// Returns an error only for critical failures. User-facing errors are
// displayed and handled gracefully within the function.
func Run(ctx context.Context) error {
	// Display application header with ASCII art and version info
	ui.PrintHeader()
	ui.PrintSeparator()

	// Initialize file system and time provider for dependency injection
	// This allows for mocking during tests
	fs := util.OSFileSystem{}
	tp := util.RealTimeProvider{}

	// Load configuration from config.json (or create default if missing)
	cfg := loadConfig()

	// Load URLs to scrape from the configured file
	urls, err := util.GetURLsFromFile(fs, cfg.UrlsFile)
	if err != nil {
		fmt.Printf("URLs could not be loaded from %s. Please check your json file.\n", cfg.UrlsFile)
		return nil
	}

	// Display current configuration to the user
	printConfig(cfg, len(urls))

	// Exit early if no URLs are configured
	if len(urls) == 0 {
		fmt.Println("⚠️ No URLs configured.")
		fmt.Printf("📄 Please add URLs to '%s' before running the scraper.\n", cfg.UrlsFile)
		return nil
	}

	ui.PrintSeparator()

	// Prompt user to choose between sequential or parallel mode
	choice := promptMode()
	ui.PrintSeparator()

	// Start timer to measure total execution time
	start := time.Now()

	// Execute the scraping operation with the selected mode
	results := runScraper(ctx, choice, urls, cfg)

	fmt.Println()

	// Display summary statistics (success rate and duration)
	printSummary(results, time.Since(start))

	ui.PrintSeparator()

	// Optionally save results to a JSON file
	promptSaveResults(fs, tp, cfg, results)
	return nil
}

// promptMode prompts the user to select a scraping mode.
// It loops until the user provides valid input (1 for Sequential, 2 for Parallel).
// Returns the selected ScrapeMode enum value.
func promptMode() ui.ScrapeMode {
	scanner := bufio.NewScanner(os.Stdin)

	// Keep prompting until valid input is received
	for {
		// Display available options to the user
		fmt.Println("Choose scraping mode:")
		fmt.Printf("%d - %s\n", ui.ModeSequential, ui.ModeSequential.String())
		fmt.Printf("%d - %s\n", ui.ModeParallel, ui.ModeParallel.String())
		ui.PrintSeparator()

		// Read user input from stdin
		scanner.Scan()
		input := scanner.Text()

		// Try to parse input as an integer
		if parsed, err := strconv.Atoi(input); err == nil {
			// Try to convert integer to a valid ScrapeMode enum
			if mode, ok := ui.ParseScrapeMode(parsed); ok {
				return mode // Valid mode selected, return it
			}
		}

		// Invalid input - show error and prompt again
		ui.PrintSeparator()
		fmt.Printf("❌ Invalid input. Please enter %d or %d.\n", ui.ModeSequential, ui.ModeParallel)
		ui.PrintSeparator()
	}
}

// runScraper executes the web scraping operation using the specified mode.
// It creates an HTTP fetcher and scraper, then runs either sequential or parallel execution
// based on the mode parameter.
//
// Sequential mode processes URLs one at a time in order.
// Parallel mode uses a worker pool to process multiple URLs concurrently.
//
// Returns a slice of Page results containing scraped data or error information.
func runScraper(ctx context.Context, mode ui.ScrapeMode, urls []string, scrapeConfig *config.ScrapeConfig) []*models.Page {
	// Create HTTP fetcher with configured timeout and user agent
	fetcher := core.NewFetcher(time.Duration(scrapeConfig.HttpTimeoutSeconds)*time.Second, scrapeConfig.UserAgent)

	// Create scraper that combines fetching and HTML parsing
	scraper := core.NewScraper(fetcher)

	// Execute based on selected mode
	switch mode {
	case ui.ModeSequential:
		// Sequential mode - process URLs one at a time
		fmt.Printf("🚀  Running %s scraper...\n", mode.String())
		ui.PrintSeparator()
		fmt.Println()
		return core.RunSequential(ctx, urls, scraper)

	case ui.ModeParallel:
		// Parallel mode - use worker pool with configured concurrency
		fmt.Printf("🚀  Running %s scraper...\n", mode.String())
		ui.PrintSeparator()
		fmt.Println()
		return core.RunParallel(ctx, urls, scraper, scrapeConfig.Concurrency)

	default:
		// Safety fallback to sequential mode (should never happen with type-safe enums)
		fmt.Printf("🚀  Running %s scraper (default)...\n", ui.ModeSequential.String())
		ui.PrintSeparator()
		fmt.Println()
		return core.RunSequential(ctx, urls, scraper)
	}
}

// printSummary displays a summary of scraping results.
// Shows the number of successful scrapes, total URLs processed, and total duration.
func printSummary(pages []*models.Page, duration time.Duration) {
	// Count how many pages were scraped successfully (no errors)
	successCount := 0
	for _, p := range pages {
		if p.Success() {
			successCount++
		}
	}

	// Display summary: success/total ratio and execution time
	fmt.Printf("👉 %d/%d successful | 🕐 Duration: %v\n", successCount, len(pages), duration)
}

// promptSaveResults prompts the user to save scraping results to a file.
// It loops until the user provides valid input (y/yes or n/no, case-insensitive).
// If the user chooses yes, results are saved as a timestamped JSON file.
func promptSaveResults(fs util.FileSystem, tp util.TimeProvider, scrapeConfig *config.ScrapeConfig, pages []*models.Page) {
	scanner := bufio.NewScanner(os.Stdin)

	// Keep prompting until valid input is received
	for {
		fmt.Print("💾  Do you want to save the results to a file? (y/n): ")
		scanner.Scan()

		// Try to parse user input as yes/no choice
		if choice, ok := ui.ParseUserChoice(scanner.Text()); ok {
			if choice.Bool() {
				// User chose yes - save results to timestamped JSON file
				filename, err := util.SaveResultsToFile(fs, tp, scrapeConfig.ResultsDirectory, pages)
				if err != nil {
					fmt.Println("🚫  Error saving file:", err)
				} else {
					fmt.Println("👉  Results saved to:", filename)
				}
			} else {
				// User chose no - skip saving
				fmt.Println("👉  Results not saved.")
			}
			return // Exit after handling valid choice
		}

		// Invalid input - show error and prompt again
		fmt.Println("❌ Please type 'y' for yes or 'n' for no")
	}
}

// printConfig displays the current scraper configuration to the user.
// Shows all relevant settings including URLs file, output directory, concurrency,
// timeout, and user agent. Long user agent strings are truncated for readability.
func printConfig(cfg *config.ScrapeConfig, urlCount int) {
	// Display main configuration settings
	fmt.Printf("📄  URLs File: %s (%d urls loaded)\n", cfg.UrlsFile, urlCount)
	fmt.Printf("💾  Results Directory: %s/\n", cfg.ResultsDirectory)
	fmt.Printf("🔧  Concurrency: %d\n", cfg.Concurrency)
	fmt.Printf("🕐  HTTP Timeout (s): %d\n", cfg.HttpTimeoutSeconds)

	// Truncate the User-Agent if it's too long for console display
	// This prevents formatting issues with very long user agent strings
	userAgent := cfg.UserAgent
	if len(userAgent) > userAgentTruncateLength {
		userAgent = userAgent[:userAgentTruncateLength] + "..."
	}

	fmt.Printf("🌐  User-Agent: %s\n", userAgent)
}

// loadConfig loads the scraper configuration from config.json.
// If the file doesn't exist or is invalid, it creates a default configuration
// and attempts to save it for future use. Always returns a valid configuration.
func loadConfig() *config.ScrapeConfig {
	// Attempt to load configuration from config.json
	cfg, err := config.LoadConfig(configFile)
	if err != nil {
		// Config file missing or invalid - create default configuration
		defaultCfg := config.NewDefaultConfig()

		// Try to save the default config for future use
		if saveErr := config.SaveConfig(configFile, defaultCfg); saveErr != nil {
			// Could not save default config to file
			fmt.Printf("⚠️  Could not load config from %s: %v\n", configFile, err)
			fmt.Println("Using default configuration (unable to save to file).")
		} else {
			// Successfully created default config file
			fmt.Printf("⚠️  Could not load config from %s: %v\n", configFile, err)
			fmt.Printf("Created default configuration file at %s\n", configFile)
		}
		ui.PrintSeparator()
		return defaultCfg
	}

	// Successfully loaded configuration from file
	return cfg
}
