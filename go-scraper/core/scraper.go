package core

import (
	"context"
	"fmt"
	"go-scraper/models"
	"io"
	"time"
)

// Scraper defines an interface for scraping a single page.
type Scraper interface {
	Scrape(ctx context.Context, url string) (*models.Page, error)
}

// DefaultScraper is the default implementation that uses a Fetcher + HTML parser.
type DefaultScraper struct {
	Fetcher HTTPFetcher
}

// NewScraper creates a DefaultScraper with the provided HTTPFetcher.
func NewScraper(fetcher HTTPFetcher) *DefaultScraper {
	return &DefaultScraper{Fetcher: fetcher}
}

// Scrape fetches the page, parses its title and links, and returns a Page model.
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
		}, fmt.Errorf("fetching %s: %w", url, err)
	}

	title, links, err := ParseHTML(bytesToReader(body))
	if err != nil {
		return &models.Page{
			URL:       url,
			Error:     fmt.Sprintf("parse failed: %v", err),
			TimeStamp: startTime,
		}, fmt.Errorf("parsing %s: %w", url, err)
	}

	return &models.Page{
		URL:       url,
		Title:     title,
		Links:     links,
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
