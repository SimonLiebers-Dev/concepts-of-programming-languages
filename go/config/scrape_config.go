package config

import (
	"encoding/json"
	"errors"
	"fmt"
	"os"
	"path/filepath"
)

const (
	// DefaultURLsFile is the default filename for URLs configuration
	DefaultURLsFile = "urls.json"
	// DefaultResultsDirectory is the default directory for output files
	DefaultResultsDirectory = "output"
	// DefaultConcurrency is the default number of parallel workers
	DefaultConcurrency = 5
	// DefaultHTTPTimeoutSeconds is the default HTTP request timeout in seconds
	DefaultHTTPTimeoutSeconds = 30
	// DefaultUserAgent is the default User-Agent header for HTTP requests
	DefaultUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/26.0 Mobile/15E148 Safari/604.1"
)

// ScrapeConfig represents the configuration settings required for the web scraper.
// It defines how the scraper should behave including concurrency limits, timeouts,
// and file locations for input/output operations.
type ScrapeConfig struct {
	UrlsFile           string `json:"urlsFile"`           // Path to JSON file containing URLs to scrape
	ResultsDirectory   string `json:"resultsDirectory"`   // Directory where scrape results will be saved
	Concurrency        int    `json:"concurrency"`        // Number of concurrent workers for parallel scraping
	HttpTimeoutSeconds int    `json:"httpTimeoutSeconds"` // HTTP request timeout in seconds
	UserAgent          string `json:"userAgent"`          // User-Agent header for HTTP requests
}

// NewDefaultConfig creates a ScrapeConfig with sensible default values.
// Use this when no configuration file exists or when you need a baseline configuration.
func NewDefaultConfig() *ScrapeConfig {
	return &ScrapeConfig{
		UrlsFile:           DefaultURLsFile,
		ResultsDirectory:   DefaultResultsDirectory,
		Concurrency:        DefaultConcurrency,
		HttpTimeoutSeconds: DefaultHTTPTimeoutSeconds,
		UserAgent:          DefaultUserAgent,
	}
}

// LoadConfig reads and parses a configuration file from the specified path.
// It validates the configuration after loading and returns an error if validation fails.
// If the file doesn't exist or is invalid, consider using NewDefaultConfig() as a fallback.
func LoadConfig(path string) (*ScrapeConfig, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read config file %s: %w", path, err)
	}

	var cfg ScrapeConfig
	if err := json.Unmarshal(data, &cfg); err != nil {
		return nil, fmt.Errorf("failed to parse config JSON from %s: %w", path, err)
	}

	if err := cfg.Validate(); err != nil {
		return nil, fmt.Errorf("config validation failed: %w", err)
	}

	return &cfg, nil
}

// SaveConfig writes a configuration object to a JSON file at the specified path.
// The configuration directory will be created if it doesn't exist.
func SaveConfig(path string, config *ScrapeConfig) error {
	if err := writeConfigFile(path, config); err != nil {
		return fmt.Errorf("failed to write config to %s: %w", path, err)
	}
	return nil
}

// Validate ensures that all configuration values are set and valid.
// It returns an error if any field is missing or has an invalid value.
func (c *ScrapeConfig) Validate() error {
	if c == nil {
		return errors.New("config cannot be nil")
	}
	if c.UrlsFile == "" {
		return errors.New("urlsFile is required")
	}
	if c.ResultsDirectory == "" {
		return errors.New("resultsDirectory is required")
	}
	if c.Concurrency <= 0 {
		return errors.New("concurrency must be greater than zero")
	}
	if c.HttpTimeoutSeconds <= 0 {
		return errors.New("httpTimeoutSeconds must be greater than zero")
	}
	if c.UserAgent == "" {
		return errors.New("userAgent is required")
	}
	return nil
}

// writeConfigFile writes a config struct to disk as JSON.
func writeConfigFile(path string, cfg *ScrapeConfig) error {
	if cfg == nil {
		return errors.New("cannot write nil config")
	}

	// Ensure directory exists
	if err := os.MkdirAll(filepath.Dir(path), 0755); err != nil {
		return fmt.Errorf("failed to create config directory: %w", err)
	}

	data, err := json.MarshalIndent(cfg, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal config: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write config file: %w", err)
	}

	return nil
}
