package core_test

import (
	"strings"
	"testing"

	"go-scraper/core"
)

func TestParseHTML(t *testing.T) {
	htmlData := `
	<html>
	  <head><title>Test Page</title></head>
	  <body>
	    <a href="https://example.com">Example</a>
	    <a href="/local/link">Local</a>
		<img src="/local/img">Local</a>
	  </body>
	</html>`

	title, links, images, err := core.ParseHTML(strings.NewReader(htmlData))
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}

	if title != "Test Page" {
		t.Errorf("expected title 'Test Page', got '%s'", title)
	}

	expectedLinks := []string{"https://example.com", "/local/link"}
	if len(links) != len(expectedLinks) {
		t.Fatalf("expected %d links, got %d", len(expectedLinks), len(links))
	}
	for i, link := range expectedLinks {
		if links[i] != link {
			t.Errorf("expected link %d to be '%s', got '%s'", i, link, links[i])
		}
	}

	expectedImages := []string{"/local/img"}
	if len(images) != len(expectedImages) {
		t.Fatalf("expected %d images, got %d", len(expectedImages), len(images))
	}
	for i, image := range expectedImages {
		if images[i] != image {
			t.Errorf("expected image %d to be '%s', got '%s'", i, image, images[i])
		}
	}
}
