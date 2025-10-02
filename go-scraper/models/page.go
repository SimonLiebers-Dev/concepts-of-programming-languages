package models

import "time"

type Page struct {
	URL       string    // The original URL of the page
	Title     string    // The title of the page
	Links     []string  // All links found on the page
	TimeStamp time.Time // When the page was scraped
	Error     string    // Error message
}
