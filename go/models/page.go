package models

import (
	"time"
)

// Page represents the result of a web scraping operation.
// It contains both successful scraping results (title, links, images) and
// error information if the operation failed. A Page is always returned even
// on failure to maintain consistent result handling.
type Page struct {
	URL       string    `json:"url"`             // The original URL that was scraped
	Title     string    `json:"title"`           // The page title extracted from <title> tag
	Links     []string  `json:"links"`           // All href attributes from <a> elements
	Images    []string  `json:"images"`          // All src attributes from <img> elements
	TimeStamp time.Time `json:"timestamp"`       // When the scraping operation started
	Error     string    `json:"error,omitempty"` // Error message if scraping failed (empty on success)
}

// HasError reports whether the page scraping encountered an error.
// Returns true if the Error field is non-empty.
func (p *Page) HasError() bool {
	return p != nil && p.Error != ""
}

// Success reports whether the page was scraped successfully without errors.
// Returns true if the Error field is empty.
func (p *Page) Success() bool {
	return p != nil && p.Error == ""
}
