package core

import (
	"context"
	"fmt"
	"io"
	"net/http"
	"time"
)

// HTTPFetcher defines an interface for fetching remote content over HTTP.
// This abstraction allows injecting mock implementations during testing,
// enabling tests to run without making actual network requests.
//
// Implementations should:
//   - Respect context cancellation and timeouts
//   - Return the raw response body as bytes
//   - Return an error if the request fails or status code is not 200 OK
type HTTPFetcher interface {
	Fetch(ctx context.Context, url string) ([]byte, error)
}

// Fetcher is the production implementation of HTTPFetcher using the standard net/http client.
// It supports configurable timeouts and custom User-Agent headers.
type Fetcher struct {
	Client    *http.Client // HTTP client with configured timeout
	UserAgent string       // User-Agent header sent with requests
}

// NewFetcher constructs a new Fetcher with the specified timeout and User-Agent.
// The timeout applies to the entire request/response cycle including connection establishment.
func NewFetcher(timeout time.Duration, userAgent string) *Fetcher {
	return &Fetcher{
		Client: &http.Client{
			Timeout: timeout,
		},
		UserAgent: userAgent,
	}
}

// Fetch performs an HTTP GET request and returns the response body as bytes.
// The context allows for cancellation and additional timeout control beyond the client timeout.
// Returns an error if the request fails, times out, or receives a non-200 status code.
func (f *Fetcher) Fetch(ctx context.Context, url string) ([]byte, error) {
	req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if err != nil {
		return nil, fmt.Errorf("failed to create HTTP request for %s: %w", url, err)
	}
	req.Header.Set("User-Agent", f.UserAgent)

	resp, err := f.Client.Do(req)
	if err != nil {
		return nil, fmt.Errorf("HTTP request failed for %s: %w", url, err)
	}
	defer func() {
		_ = resp.Body.Close()
	}()

	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("unexpected HTTP status for %s: %d %s", url, resp.StatusCode, resp.Status)
	}

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("failed to read response body from %s: %w", url, err)
	}

	return body, nil
}
