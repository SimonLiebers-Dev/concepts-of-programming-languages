package core

import (
	"io"
	"strings"

	"golang.org/x/net/html"
)

// ParseHTML extracts the <title> text and all <a href="..."> links
// from an HTML document provided via an io.Reader.
// It returns the page title (trimmed) and a slice of links.
func ParseHTML(body io.Reader) (string, []string, error) {
	doc, err := html.Parse(body)
	if err != nil {
		return "", nil, err
	}

	var title string
	var links []string

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
			}
		}
		for c := n.FirstChild; c != nil; c = c.NextSibling {
			traverse(c)
		}
	}

	traverse(doc)

	return strings.TrimSpace(title), links, nil
}
