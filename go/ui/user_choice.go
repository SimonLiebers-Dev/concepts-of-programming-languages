package ui

import "strings"

// UserChoice represents a yes/no user decision.
// This enum provides type-safe handling of binary user choices.
type UserChoice int

const (
	// ChoiceNo indicates the user declined or rejected an option.
	ChoiceNo UserChoice = 0

	// ChoiceYes indicates the user accepted or confirmed an option.
	ChoiceYes UserChoice = 1
)

// String returns a human-readable string representation of the UserChoice.
func (c UserChoice) String() string {
	switch c {
	case ChoiceYes:
		return "Yes"
	case ChoiceNo:
		return "No"
	default:
		return "Unknown"
	}
}

// IsValid reports whether the UserChoice is a valid choice value.
// Returns true only for ChoiceYes and ChoiceNo.
func (c UserChoice) IsValid() bool {
	return c == ChoiceYes || c == ChoiceNo
}

// Bool returns the boolean representation of the UserChoice.
// ChoiceYes returns true, ChoiceNo returns false.
func (c UserChoice) Bool() bool {
	return c == ChoiceYes
}

// ParseUserChoice converts a string input to a UserChoice.
// Accepts: "y", "yes", "n", "no" (case-insensitive).
// Returns the choice and true if valid, or ChoiceNo and false if invalid.
func ParseUserChoice(input string) (UserChoice, bool) {
	normalized := strings.ToLower(strings.TrimSpace(input))
	switch normalized {
	case "y", "yes":
		return ChoiceYes, true
	case "n", "no":
		return ChoiceNo, true
	default:
		return ChoiceNo, false
	}
}
