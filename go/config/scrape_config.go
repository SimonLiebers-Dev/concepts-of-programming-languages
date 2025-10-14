package config

import (
	"encoding/json"
	"errors"
	"fmt"
	"os"
	"path/filepath"
	"sync"
)

// ScrapeConfig represents the configuration settings required for the web scraper.
type ScrapeConfig struct {
	UrlsFile           string `json:"urlsFile"`
	ResultsDirectory   string `json:"resultsDirectory"`
	Concurrency        int    `json:"concurrency"`
	HttpTimeoutSeconds int    `json:"httpTimeoutSeconds"`
	UserAgent          string `json:"userAgent"`
}

var (
	instance *ScrapeConfig
	once     sync.Once
)

// NewDefaultConfig creates a default config object used if no config was configured
func NewDefaultConfig() *ScrapeConfig {
	return &ScrapeConfig{
		UrlsFile:           "urls.json",
		ResultsDirectory:   "output",
		Concurrency:        5,
		HttpTimeoutSeconds: 30,
		UserAgent:          "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/26.0 Mobile/15E148 Safari/604.1",
	}
}

// LoadConfig loads the configuration from a JSON file (singleton).
// Subsequent calls will return the same instance.
func LoadConfig(path string) (*ScrapeConfig, error) {
	var err error
	once.Do(func() {
		var cfg ScrapeConfig
		data, readErr := os.ReadFile(path)
		if readErr != nil {
			err = fmt.Errorf("failed to read config file, %w", readErr)
			return
		}

		if unmarshalErr := json.Unmarshal(data, &cfg); unmarshalErr != nil {
			err = fmt.Errorf("failed to parse config JSON, %w", unmarshalErr)
			return
		}

		if validateErr := cfg.Validate(); validateErr != nil {
			err = validateErr
			return
		}

		instance = &cfg
	})

	if err != nil {
		return nil, err
	}

	if instance == nil {
		return nil, errors.New("configuration was not initialized")
	}

	return instance, nil
}

// GetConfig returns the already loaded configuration instance.
func GetConfig() (*ScrapeConfig, error) {
	if instance == nil {
		return nil, errors.New("configuration not loaded. Call LoadConfig() first")
	}
	return instance, nil
}

// SaveConfig saves a config object to a specific path
func SaveConfig(path string, config *ScrapeConfig) error {
	if err := writeConfigFile(path, config); err != nil {
		return fmt.Errorf("failed to write default config: %w", err)
	}

	return nil
}

// Validate ensures that all configuration values are set and valid.
func (c *ScrapeConfig) Validate() error {
	if c.UrlsFile == "" {
		return errors.New("missing required configuration: scraper.urlsFile")
	}
	if c.ResultsDirectory == "" {
		return errors.New("missing required configuration: scraper.resultsDirectory")
	}
	if c.Concurrency <= 0 {
		return errors.New("scraper.concurrency must be greater than zero")
	}
	if c.HttpTimeoutSeconds <= 0 {
		return errors.New("scraper.httpTimeoutSeconds must be greater than zero")
	}
	if c.UserAgent == "" {
		return errors.New("missing required configuration: scraper.userAgent")
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
