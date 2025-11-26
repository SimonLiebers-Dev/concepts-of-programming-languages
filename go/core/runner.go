package core

import (
	"context"
	"sync"

	"go-scraper/models"
	"go-scraper/ui"
)

// RunSequential scrapes URLs one at a time in sequential order.
// Each URL is processed completely before moving to the next one.
// Progress is tracked and displayed via the UI progress bar manager.
// Context cancellation is respected - if ctx is cancelled, remaining URLs are skipped.
// Returns a slice of Page results in the same order as the input URLs.
func RunSequential(ctx context.Context, urls []string, scraper Scraper) []*models.Page {
	pbm := ui.NewProgressBarManager(len(urls))
	defer pbm.StopRenderer()

	results := make([]*models.Page, 0, len(urls))

	for _, url := range urls {
		tracker := pbm.NewTracker(url, 2)
		tracker.Increment(1) // started

		page, err := scraper.Scrape(ctx, url)
		if err != nil {
			tracker.MarkAsErrored()
		}

		results = append(results, page)
		tracker.Increment(1) // finished
	}

	return results
}

// RunParallel scrapes URLs concurrently using a worker pool pattern.
// Multiple workers process URLs in parallel up to the specified concurrency limit.
// Progress is tracked and displayed via the UI progress bar manager.
// Context cancellation is respected - workers will stop processing when ctx is cancelled.
// Returns a slice of Page results (order may differ from input URLs due to parallelism).
//
// The concurrency parameter controls the maximum number of simultaneous workers.
// If concurrency <= 0, it defaults to 1 (sequential processing).
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
