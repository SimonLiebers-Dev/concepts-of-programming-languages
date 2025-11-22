package core_test

import (
	"context"
	"errors"
	"go-scraper/core"
	"strings"
	"testing"
)

// MockFetcher for testing
type MockFetcher struct {
	Response string
	Err      error
}

func (m *MockFetcher) Fetch(_ context.Context, _ string) ([]byte, error) {
	if m.Err != nil {
		return nil, m.Err
	}
	return []byte(m.Response), nil
}

func TestScraper_Scrape_Success(t *testing.T) {
	html := `<html><head><title>Mock</title></head><body><a href="https://x.com"></a></body></html>`
	s := core.NewScraper(&MockFetcher{Response: html})

	page, err := s.Scrape(context.Background(), "https://example.com")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}

	if page.Title != "Mock" {
		t.Errorf("expected title 'Mock', got '%s'", page.Title)
	}

	if len(page.Links) != 1 || page.Links[0] != "https://x.com" {
		t.Errorf("unexpected links: %#v", page.Links)
	}
}

func TestScraper_Scrape_FetchError(t *testing.T) {
	mockErr := errors.New("network down")
	s := core.NewScraper(&MockFetcher{Err: mockErr})

	page, err := s.Scrape(context.Background(), "https://example.com")
	if err == nil {
		t.Fatal("expected error, got nil")
	}
	if page == nil {
		t.Fatal("expected page not to be nil")
	}
	if !strings.Contains(page.Error, "fetch failed") {
		t.Errorf("expected fetch error message, got: %s", page.Error)
	}
}
