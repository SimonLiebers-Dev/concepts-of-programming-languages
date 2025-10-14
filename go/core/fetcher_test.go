package core_test

import (
	"context"
	"io"
	"net/http"
	"net/http/httptest"
	"strings"
	"testing"
	"time"

	"go-scraper/core"
)

func TestFetcher_Fetch_Success(t *testing.T) {
	// Mock HTTP server
	server := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		if r.Header.Get("User-Agent") == "" {
			t.Error("expected User-Agent header to be set")
		}
		_, err := io.WriteString(w, "<html><title>Hello</title></html>")
		if err != nil {
			t.Error("html write error:", err)
		}
	}))
	defer server.Close()

	fetcher := core.NewFetcher(2*time.Second, "UserAgent")
	body, err := fetcher.Fetch(context.Background(), server.URL)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}

	if !strings.Contains(string(body), "Hello") {
		t.Errorf("expected body to contain 'Hello', got %s", string(body))
	}
}

func TestFetcher_Fetch_StatusError(t *testing.T) {
	server := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		http.Error(w, "forbidden", http.StatusForbidden)
	}))
	defer server.Close()

	fetcher := core.NewFetcher(2*time.Second, "UserAgent")
	_, err := fetcher.Fetch(context.Background(), server.URL)
	if err == nil {
		t.Fatal("expected error for non-200 response")
	}
}
