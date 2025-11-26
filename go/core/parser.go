package core

import (
	"io"
	"strings"

	"golang.org/x/net/html"
)

// ParseHTML extracts structured data from an HTML document.
// It parses the document to find:
//   - The text content of the first <title> element (trimmed of whitespace)
//   - All href attributes from <a> elements (anchor links)
//   - All src attributes from <img> elements (image sources)
//
// Returns the page title, slice of links, slice of images, and any parsing error.
// Empty strings in href or src attributes are excluded from results.
func ParseHTML(body io.Reader) (string, []string, []string, error) {
	doc, err := html.Parse(body)
	if err != nil {
		return "", nil, nil, err
	}

	var title string
	var links []string
	var images []string

	var traverse func(*html.Node)
	traverse = func(n *html.Node) {
		if n.Type == html.ElementNode {
			switch n.Data {
			case "title":
				if n.FirstChild != nil {
					title = n.FirstChild.Data
				}
			case "a":
				for _, attr := range n.Attr {
					if attr.Key == "href" && attr.Val != "" {
						links = append(links, attr.Val)
					}
				}
			case "img":
				for _, attr := range n.Attr {
					if attr.Key == "src" && attr.Val != "" {
						images = append(images, attr.Val)
					}
				}
			}
		}
		for c := n.FirstChild; c != nil; c = c.NextSibling {
			traverse(c)
		}
	}

	traverse(doc)

	return strings.TrimSpace(title), links, images, nil
}
