package ui

import "testing"

func TestUserChoice_String(t *testing.T) {
	tests := []struct {
		choice   UserChoice
		expected string
	}{
		{ChoiceYes, "Yes"},
		{ChoiceNo, "No"},
		{UserChoice(99), "Unknown"},
		{UserChoice(-1), "Unknown"},
	}

	for _, tt := range tests {
		t.Run(tt.expected, func(t *testing.T) {
			if got := tt.choice.String(); got != tt.expected {
				t.Errorf("UserChoice(%d).String() = %q, want %q", tt.choice, got, tt.expected)
			}
		})
	}
}

func TestUserChoice_IsValid(t *testing.T) {
	tests := []struct {
		choice UserChoice
		valid  bool
	}{
		{ChoiceYes, true},
		{ChoiceNo, true},
		{UserChoice(2), false},
		{UserChoice(99), false},
		{UserChoice(-1), false},
	}

	for _, tt := range tests {
		t.Run(tt.choice.String(), func(t *testing.T) {
			if got := tt.choice.IsValid(); got != tt.valid {
				t.Errorf("UserChoice(%d).IsValid() = %v, want %v", tt.choice, got, tt.valid)
			}
		})
	}
}

func TestUserChoice_Bool(t *testing.T) {
	tests := []struct {
		choice   UserChoice
		expected bool
	}{
		{ChoiceYes, true},
		{ChoiceNo, false},
	}

	for _, tt := range tests {
		t.Run(tt.choice.String(), func(t *testing.T) {
			if got := tt.choice.Bool(); got != tt.expected {
				t.Errorf("UserChoice(%d).Bool() = %v, want %v", tt.choice, got, tt.expected)
			}
		})
	}
}

func TestUserChoice_Values(t *testing.T) {
	// Ensure the enum values are as expected
	if ChoiceNo != 0 {
		t.Errorf("ChoiceNo = %d, want 0", ChoiceNo)
	}
	if ChoiceYes != 1 {
		t.Errorf("ChoiceYes = %d, want 1", ChoiceYes)
	}
}

func TestParseUserChoice(t *testing.T) {
	tests := []struct {
		input       string
		expectedOk  bool
		expectedVal UserChoice
	}{
		// Valid yes inputs
		{"y", true, ChoiceYes},
		{"Y", true, ChoiceYes},
		{"yes", true, ChoiceYes},
		{"YES", true, ChoiceYes},
		{"Yes", true, ChoiceYes},
		{" y ", true, ChoiceYes},
		{" yes ", true, ChoiceYes},

		// Valid no inputs
		{"n", true, ChoiceNo},
		{"N", true, ChoiceNo},
		{"no", true, ChoiceNo},
		{"NO", true, ChoiceNo},
		{"No", true, ChoiceNo},
		{" n ", true, ChoiceNo},
		{" no ", true, ChoiceNo},

		// Invalid inputs
		{"", false, ChoiceNo},
		{"maybe", false, ChoiceNo},
		{"1", false, ChoiceNo},
		{"0", false, ChoiceNo},
		{"yeah", false, ChoiceNo},
		{"nope", false, ChoiceNo},
		{"yep", false, ChoiceNo},
	}

	for _, tt := range tests {
		t.Run(tt.input, func(t *testing.T) {
			choice, ok := ParseUserChoice(tt.input)
			if ok != tt.expectedOk {
				t.Errorf("ParseUserChoice(%q) ok = %v, want %v", tt.input, ok, tt.expectedOk)
			}
			if choice != tt.expectedVal {
				t.Errorf("ParseUserChoice(%q) choice = %v, want %v", tt.input, choice, tt.expectedVal)
			}
		})
	}
}
