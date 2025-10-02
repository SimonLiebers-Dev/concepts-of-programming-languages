package core

import (
	"go-scraper/models"
	"go-scraper/ui"
)

// ScrapeSequential processes URLs one by one and shows progress.
func ScrapeSequential(urls []string) []*models.Page {
	// Create new progress bar manager
	pbm := ui.NewProgressBarManager(len(urls))

	// Ensure progress bar renderer stops on return
	defer pbm.StopRenderer()

	var results []*models.Page

	// Sequentially run scraping for all urls
	for _, url := range urls {
		// Create new tracker
		tracker := pbm.NewTracker(url, 2)

		// Increment by 1 -> Scraping has started
		tracker.Increment(1)

		// Scrape web page and parse content
		page, err := Scrape(url)

		// Update tracker to show error
		if err != nil {
			tracker.UpdateMessage("Error: " + err.Error())
			tracker.MarkAsErrored()
		}

		// Append scraping result to list
		results = append(results, page)

		// Increment by 1 -> Scraping is finished
		tracker.Increment(1)
	}

	// Return results
	return results
}

// ScrapeParallel processes URLs in parallel and shows progress.
func ScrapeParallel(urls []string) []*models.Page {
	// Create new progress bar manager
	pbm := ui.NewProgressBarManager(len(urls))

	// Ensure progress bar renderer stops on return
	defer pbm.StopRenderer()

	// Create channel for results
	resultChannel := make(chan *models.Page)

	// Create go routine for each url to be scraped
	for _, url := range urls {
		go func(u string) {
			// Create new tracker
			tracker := pbm.NewTracker(u, 2)

			// Increment by 1 -> Scraping has started
			tracker.Increment(1)

			// Scrape web page and parse content
			page, err := Scrape(u)

			// Update tracker to show error
			if err != nil {
				tracker.UpdateMessage("Error: " + err.Error())
				tracker.MarkAsErrored()
			}

			// Increment by 1 -> Scraping is finished
			tracker.Increment(1)

			// Add result to channel
			resultChannel <- page
		}(url)
	}

	// Create list for results
	results := make([]*models.Page, 0, len(urls))

	// Collect results from channel
	for i := 0; i < len(urls); i++ {
		page := <-resultChannel
		results = append(results, page)
	}

	// Return results
	return results
}
