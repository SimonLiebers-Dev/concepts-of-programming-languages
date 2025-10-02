package util

import (
	"encoding/json"
	"fmt"
	"go-scraper/models"
	"os"
	"time"
)

// GetURLsFromFile reads URLs from configFile, or creates it if missing.
func GetURLsFromFile(configFile string) ([]string, error) {
	// Check if file exists
	if _, err := os.Stat(configFile); os.IsNotExist(err) {
		// Create empty file
		err := os.WriteFile(configFile, []byte("[]"), 0644)
		if err != nil {
			return nil, fmt.Errorf("could not create %s: %w", configFile, err)
		}
		return []string{}, nil
	}

	// File exists -> read content
	data, err := os.ReadFile(configFile)
	if err != nil {
		return nil, fmt.Errorf("could not read urls.json: %w", err)
	}

	var urls []string
	if err := json.Unmarshal(data, &urls); err != nil {
		return nil, fmt.Errorf("invalid JSON format in urls.json: %w", err)
	}

	return urls, nil
}

// SaveResultsToFile saves the given pages to a JSON file with timestamped name.
func SaveResultsToFile(pages []*models.Page) (string, error) {
	millis := time.Now().UnixMilli()
	filename := fmt.Sprintf("scrape-results-%d.json", millis)

	data, err := json.MarshalIndent(pages, "", "  ")
	if err != nil {
		return "", fmt.Errorf("could not marshal data: %w", err)
	}

	err = os.WriteFile(filename, data, 0644)
	if err != nil {
		return "", fmt.Errorf("could not write file: %w", err)
	}

	return filename, nil
}
