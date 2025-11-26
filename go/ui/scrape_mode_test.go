package ui

import "testing"

func TestScrapeMode_String(t *testing.T) {
	tests := []struct {
		mode     ScrapeMode
		expected string
	}{
		{ModeSequential, "Sequential"},
		{ModeParallel, "Parallel"},
		{ScrapeMode(99), "Unknown"},
		{ScrapeMode(0), "Unknown"},
		{ScrapeMode(-1), "Unknown"},
	}

	for _, tt := range tests {
		t.Run(tt.expected, func(t *testing.T) {
			if got := tt.mode.String(); got != tt.expected {
				t.Errorf("ScrapeMode(%d).String() = %q, want %q", tt.mode, got, tt.expected)
			}
		})
	}
}

func TestScrapeMode_IsValid(t *testing.T) {
	tests := []struct {
		mode  ScrapeMode
		valid bool
	}{
		{ModeSequential, true},
		{ModeParallel, true},
		{ScrapeMode(0), false},
		{ScrapeMode(3), false},
		{ScrapeMode(99), false},
		{ScrapeMode(-1), false},
	}

	for _, tt := range tests {
		t.Run(tt.mode.String(), func(t *testing.T) {
			if got := tt.mode.IsValid(); got != tt.valid {
				t.Errorf("ScrapeMode(%d).IsValid() = %v, want %v", tt.mode, got, tt.valid)
			}
		})
	}
}

func TestParseScrapeMode(t *testing.T) {
	tests := []struct {
		input       int
		expectedOk  bool
		expectedVal ScrapeMode
	}{
		{1, true, ModeSequential},
		{2, true, ModeParallel},
		{0, false, ScrapeMode(0)},
		{3, false, ScrapeMode(3)},
		{-1, false, ScrapeMode(-1)},
		{99, false, ScrapeMode(99)},
	}

	for _, tt := range tests {
		t.Run(tt.expectedVal.String(), func(t *testing.T) {
			mode, ok := ParseScrapeMode(tt.input)
			if ok != tt.expectedOk {
				t.Errorf("ParseScrapeMode(%d) ok = %v, want %v", tt.input, ok, tt.expectedOk)
			}
			if mode != tt.expectedVal {
				t.Errorf("ParseScrapeMode(%d) mode = %v, want %v", tt.input, mode, tt.expectedVal)
			}
		})
	}
}
