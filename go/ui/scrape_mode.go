package ui

// ScrapeMode represents the execution mode for the web scraper.
// Users can choose between sequential and parallel execution.
type ScrapeMode int

const (
	// ModeSequential indicates URLs should be scraped one at a time in order.
	// This mode is easier to debug and uses less system resources.
	ModeSequential ScrapeMode = 1

	// ModeParallel indicates URLs should be scraped concurrently using a worker pool.
	// This mode is faster for multiple URLs but uses more system resources.
	ModeParallel ScrapeMode = 2
)

// String returns a human-readable string representation of the ScrapeMode.
// This is useful for logging, debugging, and user-facing messages.
func (m ScrapeMode) String() string {
	switch m {
	case ModeSequential:
		return "Sequential"
	case ModeParallel:
		return "Parallel"
	default:
		return "Unknown"
	}
}

// IsValid reports whether the ScrapeMode is a valid mode value.
// Returns true only for ModeSequential and ModeParallel.
func (m ScrapeMode) IsValid() bool {
	return m == ModeSequential || m == ModeParallel
}

// ParseScrapeMode converts an integer input to a ScrapeMode.
// Returns the mode and true if valid, or zero value and false if invalid.
func ParseScrapeMode(input int) (ScrapeMode, bool) {
	mode := ScrapeMode(input)
	return mode, mode.IsValid()
}
