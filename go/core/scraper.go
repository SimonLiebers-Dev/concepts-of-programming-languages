package core

import (
	"context"
	"fmt"
	"go-scraper/models"
	"io"
	"time"
)

// Scraper defines an interface for scraping a single web page.
// This abstraction allows injecting mock implementations during testing,
// enabling tests without making actual HTTP requests or parsing HTML.
//
// Implementations should:
//   - Respect context cancellation and timeouts
//   - Return a Page object containing extracted data (title, links, images)
//   - Populate the Page.Error field if scraping fails, rather than returning nil
//   - Return both a Page and an error to provide structured data even on failure
type Scraper interface {
	Scrape(ctx context.Context, url string) (*models.Page, error)
}

// DefaultScraper is the production implementation that combines HTTP fetching
// with HTML parsing to extract structured data from web pages.
type DefaultScraper struct {
	Fetcher HTTPFetcher // HTTPFetcher implementation for retrieving page content
}

// NewScraper creates a DefaultScraper with the provided HTTPFetcher.
// The fetcher will be used to retrieve page content before parsing.
func NewScraper(fetcher HTTPFetcher) *DefaultScraper {
	return &DefaultScraper{Fetcher: fetcher}
}

// Scrape fetches a web page, parses its HTML content, and returns a Page model
// containing the extracted title, links, and images. The Page.Error field is
// populated if fetching or parsing fails. An error is also returned for
// programmatic error handling.
func (s *DefaultScraper) Scrape(ctx context.Context, url string) (*models.Page, error) {
	startTime := time.Now()

	// Defensive: ensure fetcher is initialized
	if s.Fetcher == nil {
		return &models.Page{
			URL:       url,
			Error:     "no fetcher configured",
			TimeStamp: startTime,
		}, fmt.Errorf("scraper misconfiguration: no fetcher provided")
	}

	body, err := s.Fetcher.Fetch(ctx, url)
	if err != nil {
		return &models.Page{
			URL:       url,
			Error:     fmt.Sprintf("fetch failed: %v", err),
			TimeStamp: startTime,
		}, fmt.Errorf("failed to fetch %s: %w", url, err)
	}

	title, links, images, err := ParseHTML(bytesToReader(body))
	if err != nil {
		return &models.Page{
			URL:       url,
			Error:     fmt.Sprintf("parse failed: %v", err),
			TimeStamp: startTime,
		}, fmt.Errorf("failed to parse HTML from %s: %w", url, err)
	}

	return &models.Page{
		URL:       url,
		Title:     title,
		Links:     links,
		Images:    images,
		TimeStamp: time.Now(),
	}, nil
}

// bytesToReader converts a byte slice into an io.Reader.
func bytesToReader(b []byte) *readerWrapper {
	return &readerWrapper{data: b}
}

type readerWrapper struct {
	data []byte
	pos  int
}

func (r *readerWrapper) Read(p []byte) (int, error) {
	if r.pos >= len(r.data) {
		return 0, io.EOF
	}
	n := copy(p, r.data[r.pos:])
	r.pos += n
	return n, nil
}
