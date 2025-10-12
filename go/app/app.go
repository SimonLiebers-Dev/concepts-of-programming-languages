package app

import (
	"bufio"
	"context"
	"fmt"
	"go-scraper/core"
	"go-scraper/models"
	"go-scraper/ui"
	"go-scraper/util"
	"os"
	"strconv"
	"strings"
	"time"
)

const urlsFile = "urls.json"

// Run is the top-level entry point for the scraper application.
// It handles user interaction and orchestrates scraping.
func Run(ctx context.Context) error {
	ui.PrintHeader()
	ui.PrintSeparator()

	fs := util.OSFileSystem{}
	tp := util.RealTimeProvider{}

	urls, err := util.GetURLsFromFile(fs, urlsFile)
	if err != nil {
		return fmt.Errorf("loading URLs: %w", err)
	}

	if len(urls) == 0 {
		fmt.Println("⚠️ No URLs configured.")
		fmt.Println("📄 Please add URLs to 'urls.json' before running the scraper.")
		return nil
	}

	fmt.Printf("Loaded %d URLs from %s\n", len(urls), urlsFile)
	ui.PrintSeparator()

	choice := promptMode()
	ui.PrintSeparator()

	start := time.Now()
	results := runScraper(ctx, choice, urls)
	printSummary(results, time.Since(start))
	ui.PrintSeparator()

	promptSaveResults(fs, tp, results)
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

func runScraper(ctx context.Context, choice int, urls []string) []*models.Page {
	fetcher := core.NewFetcher(15 * time.Second)
	scraper := core.NewScraper(fetcher)

	switch choice {
	case 1:
		fmt.Println("🚀 Running sequential scraper...")
		ui.PrintSeparator()
		return core.RunSequential(ctx, urls, scraper)
	default:
		fmt.Println("🚀 Running parallel scraper...")
		ui.PrintSeparator()
		return core.RunParallel(ctx, urls, scraper, 5)
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

func promptSaveResults(fs util.FileSystem, tp util.TimeProvider, pages []*models.Page) {
	scanner := bufio.NewScanner(os.Stdin)
	for {
		fmt.Print("💾 Do you want to save the results to a file? (y/n): ")
		scanner.Scan()

		answer := strings.TrimSpace(strings.ToLower(scanner.Text()))
		switch answer {
		case "y":
			filename, err := util.SaveResultsToFile(fs, tp, pages)
			if err != nil {
				fmt.Println("❌ Error saving file:", err)
			} else {
				fmt.Println("✅ Results saved to:", filename)
			}
			return
		case "n":
			fmt.Println("ℹ️ Results not saved.")
			return
		default:
			fmt.Println("Please type 'y' or 'n'")
		}
	}
}
