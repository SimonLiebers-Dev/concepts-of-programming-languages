package core

import (
	"go-scraper/models"
	"go-scraper/ui"
	"sync"
)

// ScrapeSequential processes URLs one by one and shows progress.
func ScrapeSequential(urls []string) []*models.Page {
	pbm := ui.NewProgressBarManager(len(urls))
	defer pbm.StopRenderer()

	var results []*models.Page

	for _, url := range urls {
		tracker := pbm.NewTracker(url, 2) // Fetch + Parse
		tracker.Increment(1)              // Simulate fetch

		page, err := Scrape(url)
		if err != nil {
			tracker.UpdateMessage("Error: " + err.Error())
			tracker.MarkAsErrored()
		}

		results = append(results, page)
		tracker.Increment(1)
	}

	return results
}

// ScrapeParallel processes URLs in parallel and shows progress.
func ScrapeParallel(urls []string) []*models.Page {
	pbm := ui.NewProgressBarManager(len(urls))
	defer pbm.StopRenderer()

	var (
		results []*models.Page
		mutex   sync.Mutex
		wg      sync.WaitGroup
	)

	wg.Add(len(urls))
	for _, url := range urls {
		go func(u string) {
			defer wg.Done()

			tracker := pbm.NewTracker(u, 2)
			tracker.Increment(1) // Simulate fetch

			page, err := Scrape(u)
			if err != nil {
				tracker.UpdateMessage("Error: " + err.Error())
				tracker.MarkAsErrored()
			}

			mutex.Lock()
			results = append(results, page)
			mutex.Unlock()

			tracker.Increment(1)
		}(url)
	}

	wg.Wait()

	return results
}
