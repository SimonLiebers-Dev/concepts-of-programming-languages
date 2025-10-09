package core_test

import (
	"context"
	"go-scraper/core"
	"go-scraper/models"
	"testing"
)

type MockScraper struct{}

func (MockScraper) Scrape(ctx context.Context, url string) (*models.Page, error) {
	return &models.Page{URL: url, Title: "OK"}, nil
}

func TestRunSequential(t *testing.T) {
	urls := []string{"a", "b", "c"}
	results := core.RunSequential(context.Background(), urls, MockScraper{})

	if len(results) != len(urls) {
		t.Fatalf("expected %d results, got %d", len(urls), len(results))
	}
}

func TestRunParallel(t *testing.T) {
	urls := []string{"a", "b", "c"}
	results := core.RunParallel(context.Background(), urls, MockScraper{}, 2)

	if len(results) != len(urls) {
		t.Fatalf("expected %d results, got %d", len(urls), len(results))
	}
}
