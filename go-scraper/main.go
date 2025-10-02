package main

import (
	"bufio"
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

const UrlsFile = "urls.json"

func main() {
	ui.PrintHeader()
	ui.PrintSeparator()

	urls, err := util.GetURLsFromFile(UrlsFile)
	if err != nil {
		fmt.Println("❌ Error loading URLs:", err)
		return
	}

	if len(urls) == 0 {
		fmt.Println("⚠️  No URLs configured.")
		fmt.Println("📄 Please add URLs to 'urls.json' before running the scraper.")
		return
	}

	fmt.Printf("Loaded %d URLs from %s\n", len(urls), UrlsFile)

	ui.PrintSeparator()

	// Create new scanner
	scanner := bufio.NewScanner(os.Stdin)

	// Init variable for storing menu choice
	var choice int

	// Start loop to prompt menu choice from user
	for {
		// Show menu with scraping options
		fmt.Println("Choose scraping mode:")
		fmt.Println("1 - Sequential")
		fmt.Println("2 - Parallel")
		ui.PrintSeparator()

		// Scan text input
		scanner.Scan()

		// Read input
		input := strings.TrimSpace(scanner.Text())

		// Try parse input to int
		parsed, err := strconv.Atoi(input)

		// Check if error was returned
		if err != nil {
			// Print error message and formatting
			ui.PrintSeparator()
			fmt.Println("❌ Invalid input. Please enter 1 or 2.")
			ui.PrintSeparator()

			// Skip iteration to prompt input again
			continue
		}

		// Check if input matches one of the available options (1 or 2)
		if parsed != 1 && parsed != 2 {
			// Print error message and formatting
			ui.PrintSeparator()
			fmt.Println("❌ Invalid choice. Please enter 1 or 2.")
			ui.PrintSeparator()

			// Skip iteration to prompt input again
			continue
		}

		// Store choice in outside variable
		choice = parsed

		// Break loop to start scraping with chosen mode
		break
	}

	// Formatting
	ui.PrintSeparator()

	// Initialize array for scraping result
	var pages []*models.Page

	// Store timestamp before scraping to calculate duration
	start := time.Now()

	// Run chosen scraping mode
	switch choice {
	case 1:
		fmt.Println("🚀 Running sequential scraper...")
		ui.PrintSeparator()

		// Run sequential scraping
		pages = core.ScrapeSequential(urls)
	default:
		fmt.Println("🚀 Running parallel scraper...")
		ui.PrintSeparator()

		// Run parallel scraping
		pages = core.ScrapeParallel(urls)
	}

	// Count successes and errors
	successCount := 0
	errorCount := 0
	for _, p := range pages {
		if p.Error != "" {
			errorCount++
		} else {
			successCount++
		}
	}

	// Print information about number of results and duration
	fmt.Printf("✅ %d successful | ❌ %d failed | ⏱️ Duration: %v\n", successCount, errorCount, time.Since(start))

	// Print separator
	ui.PrintSeparator()

	// Prompt for saving results
	for {
		fmt.Print("💾 Do you want to save the results to a file? (y/n): ")
		scanner.Scan()

		// Read user input and format to lower case
		answer := strings.TrimSpace(strings.ToLower(scanner.Text()))

		// Check answer
		if answer == "y" {
			// Save results to file
			filename, err := util.SaveResultsToFile(pages)

			// Check if saving did work
			if err != nil {
				// Print error message
				fmt.Println("❌ Error saving file:", err)
			} else {
				// Print success message
				fmt.Println("✅ Results saved to:", filename)
			}
			break
		} else if answer == "n" {
			// Print information, that result was not saved
			fmt.Println("ℹ️  Results not saved.")
			break
		} else {
			// Input did not match y/n -> Stay in loop and prompt again
			fmt.Println("Please type 'y' or 'n'")
		}
	}
}
