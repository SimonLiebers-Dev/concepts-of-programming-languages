package core

import (
	"context"
	"fmt"
	"io"
	"net/http"
	"time"
)

// HTTPFetcher defines an interface for fetching remote content.
type HTTPFetcher interface {
	Fetch(ctx context.Context, url string) ([]byte, error)
}

// Fetcher implements HTTPFetcher using the standard net/http client.
type Fetcher struct {
	Client    *http.Client
	UserAgent string
}

// NewFetcher constructs a new Fetcher.
func NewFetcher(timeout time.Duration, userAgent string) *Fetcher {
	return &Fetcher{
		Client: &http.Client{
			Timeout: timeout,
		},
		UserAgent: userAgent,
	}
}

// Fetch performs an HTTP GET request and returns the response body as bytes.
// The context allows for cancellation and timeouts.
func (f *Fetcher) Fetch(ctx context.Context, url string) ([]byte, error) {
	req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if err != nil {
		return nil, fmt.Errorf("creating request: %w", err)
	}
	req.Header.Set("User-Agent", f.UserAgent)

	resp, err := f.Client.Do(req)
	if err != nil {
		return nil, fmt.Errorf("fetching %s: %w", url, err)
	}
	defer func() {
		_ = resp.Body.Close()
	}()

	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("fetching %s: unexpected status %d %s", url, resp.StatusCode, resp.Status)
	}

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("reading body from %s: %w", url, err)
	}

	return body, nil
}
