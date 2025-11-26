package util

import (
	"encoding/json"
	"fmt"
	"go-scraper/models"
	"os"
	"path/filepath"
	"time"
)

// FileSystem defines an interface for filesystem operations.
// This abstraction allows injecting mock implementations during testing,
// making file I/O operations testable without touching the actual filesystem.
//
// Implementations should provide:
//   - Stat: Check if a file or directory exists and get its metadata
//   - ReadFile: Read entire file contents into memory
//   - WriteFile: Write data to a file with specified permissions
//   - MakeDir: Create a directory and all necessary parent directories
type FileSystem interface {
	Stat(name string) (os.FileInfo, error)
	ReadFile(name string) ([]byte, error)
	WriteFile(name string, data []byte, perm os.FileMode) error
	MakeDir(path string) error
}

// OSFileSystem is the production implementation of FileSystem that delegates
// to the standard library's os package for actual filesystem operations.
type OSFileSystem struct{}

// Stat returns file information for the given path.
func (OSFileSystem) Stat(name string) (os.FileInfo, error) { return os.Stat(name) }

// ReadFile reads the entire contents of a file.
func (OSFileSystem) ReadFile(name string) ([]byte, error) { return os.ReadFile(name) }

// WriteFile writes data to a file, creating it if necessary.
func (OSFileSystem) WriteFile(name string, data []byte, perm os.FileMode) error {
	return os.WriteFile(name, data, perm)
}

// MakeDir creates a directory and all necessary parent directories.
func (OSFileSystem) MakeDir(path string) error {
	return os.MkdirAll(path, os.ModePerm)
}

// TimeProvider abstracts time generation for deterministic testing.
// This allows tests to control timestamps without relying on the system clock.
//
// Implementations should provide:
//   - NowUnixMilli: Return current time as Unix milliseconds since epoch
type TimeProvider interface {
	NowUnixMilli() int64
}

// RealTimeProvider is the production implementation of TimeProvider
// that uses the system clock via time.Now().
type RealTimeProvider struct{}

// NowUnixMilli returns the current time as Unix milliseconds.
func (RealTimeProvider) NowUnixMilli() int64 { return time.Now().UnixMilli() }

// GetURLsFromFile reads URLs from a JSON file. If the file doesn't exist,
// it creates an empty JSON array file and returns an empty slice.
// Returns an error if the file cannot be read or contains invalid JSON.
func GetURLsFromFile(fs FileSystem, configFile string) ([]string, error) {
	if _, err := fs.Stat(configFile); os.IsNotExist(err) {
		// File doesn't exist, create empty JSON array
		if createErr := fs.WriteFile(configFile, []byte("[]"), 0644); createErr != nil {
			return nil, fmt.Errorf("failed to create URLs file %s: %w", configFile, createErr)
		}
		return []string{}, nil
	}

	data, err := fs.ReadFile(configFile)
	if err != nil {
		return nil, fmt.Errorf("failed to read URLs from %s: %w", configFile, err)
	}

	var urls []string
	if err := json.Unmarshal(data, &urls); err != nil {
		return nil, fmt.Errorf("invalid JSON format in %s: %w", configFile, err)
	}

	return urls, nil
}

// SaveResultsToFile saves the given pages to a timestamped JSON file inside the specified folder.
// It uses injected filesystem and time providers for testability.
// The filename format is "scrape-results-{timestamp}.json" where timestamp is Unix milliseconds.
// Returns the full path to the saved file or an error if the operation fails.
func SaveResultsToFile(fs FileSystem, tp TimeProvider, folder string, pages []*models.Page) (string, error) {
	if len(pages) == 0 {
		return "", fmt.Errorf("cannot save results: no pages provided")
	}

	// Ensure target folder exists
	if err := fs.MakeDir(folder); err != nil {
		return "", fmt.Errorf("failed to create output directory %s: %w", folder, err)
	}

	// Build full file path with timestamp
	filename := fmt.Sprintf("scrape-results-%d.json", tp.NowUnixMilli())
	fullPath := filepath.Join(folder, filename)

	// Serialize to JSON with indentation
	data, err := json.MarshalIndent(pages, "", "  ")
	if err != nil {
		return "", fmt.Errorf("failed to serialize results to JSON: %w", err)
	}

	// Write file to disk
	if err := fs.WriteFile(fullPath, data, 0644); err != nil {
		return "", fmt.Errorf("failed to write results to %s: %w", fullPath, err)
	}

	return fullPath, nil
}
