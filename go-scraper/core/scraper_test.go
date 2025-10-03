package core

import (
	"strings"
	"testing"
)

func TestParseHtml_ValidHtml(t *testing.T) {
	// Arrange
	html := `
		<html>
			<head><title> Test Page </title></head>
			<body>
				<a href="https://example.com">Example</a>
				<a href="/local">Local</a>
				<a>No href</a>
				<a href="">Empty href</a>
			</body>
		</html>
	`

	// Act
	title, links, err := parseHtml(strings.NewReader(html))

	// Assert: Error should be nil
	if err != nil {
		t.Fatalf("Unexpected error: %v", err)
	}

	// Assert: Check title
	expectedTitle := "Test Page"
	if title != expectedTitle {
		t.Errorf("Expected title '%s', got '%s'", expectedTitle, title)
	}

	// Assert: Check links
	expectedLinks := []string{"https://example.com", "/local"}
	if len(links) != len(expectedLinks) {
		t.Fatalf("Expected %d links, got %d", len(expectedLinks), len(links))
	}

	for _, want := range expectedLinks {
		found := false
		for _, got := range links {
			if got == want {
				found = true
				break
			}
		}
		if !found {
			t.Errorf("Expected link %s not found", want)
		}
	}
}