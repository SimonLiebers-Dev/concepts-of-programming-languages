package models

import (
	"time"
)

// Page represents the result of a web scraping operation.
// It contains the URL, parsed title, extracted links, timestamp, and any error message.
type Page struct {
	URL       string    `json:"url"`             // The original URL of the page
	Title     string    `json:"title"`           // The title of the page
	Links     []string  `json:"links"`           // All links found on the page
	Images    []string  `json:"images"`          // All images found on the page
	TimeStamp time.Time `json:"timestamp"`       // When the page was scraped
	Error     string    `json:"error,omitempty"` // Optional error message if something failed
}

// HasError reports whether the page contains an error message.
func (p *Page) HasError() bool {
	return p != nil && p.Error != ""
}

// Success reports whether the scrape completed successfully.
func (p *Page) Success() bool {
	return p != nil && p.Error == ""
}
