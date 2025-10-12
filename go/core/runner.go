package core

import (
	"context"
	"sync"

	"go-scraper/models"
	"go-scraper/ui"
)

// RunSequential scrapes URLs one by one and updates the UI progress.
func RunSequential(ctx context.Context, urls []string, scraper Scraper) []*models.Page {
	pbm := ui.NewProgressBarManager(len(urls))
	defer pbm.StopRenderer()

	results := make([]*models.Page, 0, len(urls))

	for _, url := range urls {
		tracker := pbm.NewTracker(url, 2)
		tracker.Increment(1) // started

		page, err := scraper.Scrape(ctx, url)
		if err != nil {
			tracker.UpdateMessage("Error: " + err.Error())
			tracker.MarkAsErrored()
		}

		results = append(results, page)
		tracker.Increment(1) // finished
	}

	return results
}

// RunParallel scrapes URLs concurrently using a classic worker pool pattern.
func RunParallel(ctx context.Context, urls []string, scraper Scraper, concurrency int) []*models.Page {
	// Enforce minimal concurrency of 1
	if concurrency <= 0 {
		concurrency = 1
	}

	pbm := ui.NewProgressBarManager(len(urls))
	defer pbm.StopRenderer()

	jobs := make(chan int, len(urls))
	results := make(chan *models.Page, len(urls))

	// Define worker
	worker := func(id int, jobs <-chan int, results chan<- *models.Page) {
		for i := range jobs {
			select {
			case <-ctx.Done():
				return // stop early if canceled
			default:
				url := urls[i]
				tracker := pbm.NewTracker(url, 2)
				tracker.Increment(1)

				page, err := scraper.Scrape(ctx, url)
				if err != nil {
					tracker.UpdateMessage("Error: " + err.Error())
					tracker.MarkAsErrored()
				}

				tracker.Increment(1)

				results <- page
			}
		}
	}

	// Start fixed number of workers
	var wg sync.WaitGroup
	for w := 1; w <= concurrency; w++ {
		wg.Add(1)
		go func(id int) {
			defer wg.Done()
			worker(id, jobs, results)
		}(w)
	}

	// Send jobs
	for i := range urls {
		jobs <- i
	}
	close(jobs)

	// Wait for workers and then close results
	go func() {
		wg.Wait()
		close(results)
	}()

	// Collect results
	pages := make([]*models.Page, 0, len(urls))
	for page := range results {
		pages = append(pages, page)
	}

	return pages
}
