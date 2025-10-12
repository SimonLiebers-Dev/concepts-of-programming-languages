package util

import (
	"encoding/json"
	"fmt"
	"go-scraper/models"
	"os"
	"time"
)

// FileSystem defines an interface for filesystem operations.
// This allows injecting mocks for testing.
type FileSystem interface {
	Stat(name string) (os.FileInfo, error)
	ReadFile(name string) ([]byte, error)
	WriteFile(name string, data []byte, perm os.FileMode) error
}

// OSFileSystem is the real implementation that uses the os package.
type OSFileSystem struct{}

func (OSFileSystem) Stat(name string) (os.FileInfo, error) { return os.Stat(name) }
func (OSFileSystem) ReadFile(name string) ([]byte, error)  { return os.ReadFile(name) }
func (OSFileSystem) WriteFile(name string, data []byte, perm os.FileMode) error {
	return os.WriteFile(name, data, perm)
}

// TimeProvider abstracts time generation for deterministic tests.
type TimeProvider interface {
	NowUnixMilli() int64
}

// RealTimeProvider is the default implementation using time.Now.
type RealTimeProvider struct{}

func (RealTimeProvider) NowUnixMilli() int64 { return time.Now().UnixMilli() }

// GetURLsFromFile reads URLs from a config file or creates it if missing.
func GetURLsFromFile(fs FileSystem, configFile string) ([]string, error) {
	if _, err := fs.Stat(configFile); os.IsNotExist(err) {
		if err := fs.WriteFile(configFile, []byte("[]"), 0644); err != nil {
			return nil, fmt.Errorf("could not create %s: %w", configFile, err)
		}
		return []string{}, nil
	}

	data, err := fs.ReadFile(configFile)
	if err != nil {
		return nil, fmt.Errorf("could not read %s: %w", configFile, err)
	}

	var urls []string
	if err := json.Unmarshal(data, &urls); err != nil {
		return nil, fmt.Errorf("invalid JSON in %s: %w", configFile, err)
	}

	return urls, nil
}

// SaveResultsToFile saves the given pages to a timestamped JSON file.
// It uses injected filesystem and time providers for testability.
func SaveResultsToFile(fs FileSystem, tp TimeProvider, pages []*models.Page) (string, error) {
	if len(pages) == 0 {
		return "", fmt.Errorf("no pages to save")
	}

	filename := fmt.Sprintf("scrape-results-%d.json", tp.NowUnixMilli())

	data, err := json.MarshalIndent(pages, "", "  ")
	if err != nil {
		return "", fmt.Errorf("could not marshal data: %w", err)
	}

	if err := fs.WriteFile(filename, data, 0644); err != nil {
		return "", fmt.Errorf("could not write file: %w", err)
	}

	return filename, nil
}
