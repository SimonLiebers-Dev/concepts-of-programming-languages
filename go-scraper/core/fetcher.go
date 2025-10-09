package core

import (
	"context"
	"fmt"
	"io"
	"net/http"
	"time"
)

// DefaultUserAgent is used if none is explicitly provided.
const DefaultUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.6.2 Mobile/15E148 Safari/604.1"

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
func NewFetcher(timeout time.Duration) *Fetcher {
	return &Fetcher{
		Client: &http.Client{
			Timeout: timeout,
		},
		UserAgent: DefaultUserAgent,
	}
}

// Fetch performs an HTTP GET request and returns the response body as bytes.
// The context allows for cancellation and timeouts.
func (f *Fetcher) Fetch(ctx context.Context, url string) ([]byte, error) {
	if f.Client == nil {
		f.Client = &http.Client{Timeout: 10 * time.Second}
	}
	if f.UserAgent == "" {
		f.UserAgent = DefaultUserAgent
	}

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
