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
	"strings"
	"time"
)

const configFile = "config.json"

// Run is the top-level entry point for the scraper application.
// It handles user interaction and orchestrates scraping.
func Run(ctx context.Context) error {
	ui.PrintHeader()
	ui.PrintSeparator()

	fs := util.OSFileSystem{}
	tp := util.RealTimeProvider{}

	cfg := loadConfig()

	urls, err := util.GetURLsFromFile(fs, cfg.UrlsFile)
	if err != nil {
		fmt.Printf("URLs could not be loaded from %s. Please check your json file.\n", cfg.UrlsFile)
		return nil
	}

	printConfig(cfg, len(urls))

	if len(urls) == 0 {
		fmt.Println("⚠️ No URLs configured.")
		fmt.Printf("📄 Please add URLs to '%s' before running the scraper.\n", cfg.UrlsFile)
		return nil
	}

	ui.PrintSeparator()

	choice := promptMode()
	ui.PrintSeparator()

	start := time.Now()
	results := runScraper(ctx, choice, urls, cfg)
	printSummary(results, time.Since(start))
	ui.PrintSeparator()

	promptSaveResults(fs, tp, cfg, results)
	return nil
}

func promptMode() int {
	scanner := bufio.NewScanner(os.Stdin)
	for {
		fmt.Println("Choose scraping mode:")
		fmt.Println("1 - Sequential")
		fmt.Println("2 - Parallel")
		ui.PrintSeparator()

		scanner.Scan()
		input := strings.TrimSpace(scanner.Text())

		if parsed, err := strconv.Atoi(input); err == nil && (parsed == 1 || parsed == 2) {
			return parsed
		}

		ui.PrintSeparator()
		fmt.Println("❌ Invalid input. Please enter 1 or 2.")
		ui.PrintSeparator()
	}
}

func runScraper(ctx context.Context, choice int, urls []string, scrapeConfig *config.ScrapeConfig) []*models.Page {
	fetcher := core.NewFetcher(time.Duration(scrapeConfig.HttpTimeoutSeconds)*time.Second, scrapeConfig.UserAgent)
	scraper := core.NewScraper(fetcher)

	switch choice {
	case 1:
		fmt.Println("🚀  Running sequential scraper...")
		ui.PrintSeparator()
		return core.RunSequential(ctx, urls, scraper)
	default:
		fmt.Println("🚀  Running parallel scraper...")
		ui.PrintSeparator()
		return core.RunParallel(ctx, urls, scraper, scrapeConfig.Concurrency)
	}
}

func printSummary(pages []*models.Page, duration time.Duration) {
	success, fail := 0, 0
	for _, p := range pages {
		if p == nil {
			fail++
			continue
		}
		if p.Error != "" {
			fail++
		} else {
			success++
		}
	}
	fmt.Printf("✅ %d successful | ❌ %d failed | ⏱️ Duration: %v\n", success, fail, duration)
}

func promptSaveResults(fs util.FileSystem, tp util.TimeProvider, scrapeConfig *config.ScrapeConfig, pages []*models.Page) {
	scanner := bufio.NewScanner(os.Stdin)
	for {
		fmt.Print("💾  Do you want to save the results to a file? (y/n): ")
		scanner.Scan()

		answer := strings.TrimSpace(strings.ToLower(scanner.Text()))
		switch answer {
		case "y":
			filename, err := util.SaveResultsToFile(fs, tp, scrapeConfig.ResultsDirectory, pages)
			if err != nil {
				fmt.Println("❌ Error saving file:", err)
			} else {
				fmt.Println("✅ Results saved to:", filename)
			}
			return
		case "n":
			fmt.Println("ℹ️  Results not saved.")
			return
		default:
			fmt.Println("Please type 'y' or 'n'")
		}
	}
}

func printConfig(cfg *config.ScrapeConfig, urlCount int) {
	fmt.Printf("📄  URLs File: %s (%d urls loaded)\n", cfg.UrlsFile, urlCount)
	fmt.Printf("💾  Results Directory: %s/\n", cfg.ResultsDirectory)
	fmt.Printf("⚙️  Concurrency: %d\n", cfg.Concurrency)
	fmt.Printf("⏱️  HTTP Timeout (s): %d\n", cfg.HttpTimeoutSeconds)

	// Truncate the User-Agent if it's longer than 80 characters
	userAgent := cfg.UserAgent
	if len(userAgent) > 80 {
		userAgent = userAgent[:80] + "..."
	}

	fmt.Printf("🕸️  User-Agent: %s\n", userAgent)
}

func loadConfig() *config.ScrapeConfig {
	cfg, err := config.LoadConfig(configFile)
	if err != nil {
		defaultCfg := config.NewDefaultConfig()
		err = config.SaveConfig(configFile, defaultCfg)

		if err != nil {
			fmt.Println("No config file was found. Fallback to default values.")
			ui.PrintSeparator()
			return defaultCfg
		} else {
			fmt.Println("Invalid config or no config file was found. The file was created with default values in", configFile)
			ui.PrintSeparator()
			return defaultCfg
		}
	} else {
		return cfg
	}
}
