package core

import (
	"fmt"
	"go-scraper/models"
	"io"
	"net/http"
	"strings"
	"time"

	"golang.org/x/net/html"
)

const UserAgent = "Mozilla/5.0 (iPad; U; CPU OS 4_3_2 like Mac OS X; en-us) AppleWebKit/533.17.9 (KHTML, like Gecko) Mobile/8H7"

func Scrape(url string) (*models.Page, error) {
	errorResult := &models.Page{
		TimeStamp: time.Now(),
	}

	client := &http.Client{}
	req, createReqError := http.NewRequest("GET", url, nil)

	if createReqError != nil {
		errorResult.Error = "Failed to create request: " + createReqError.Error()
    	return errorResult, fmt.Errorf("failed to create request: %w", createReqError)
	}

	req.Header.Set("User-Agent", UserAgent)
	resp, err := client.Do(req)

	if err != nil {
		errorResult.Error = "Failed to fetch url: " + err.Error()
		return errorResult, fmt.Errorf("failed to fetch URL %s: %w", url, err)
	}

	defer func() {
		if closeError := resp.Body.Close(); closeError != nil {
			fmt.Printf("⚠️ Warning: error closing response body from %s: %v\n", url, closeError)
		}
	}()

	if resp.StatusCode != http.StatusOK {
		errorResult.Error = "Status code " + resp.Status
		return errorResult, fmt.Errorf("failed to fetch URL %s: status code %d", url, resp.StatusCode)
	}

	title, links, err := parseHtml(resp.Body)
	if err != nil {
		errorResult.Error = "Failed to parse html: " + err.Error()
		return errorResult, fmt.Errorf("failed to parse HTML from %s: %w", url, err)
	}

	page := &models.Page{
		URL:       url,
		Title:     title,
		Links:     links,
		TimeStamp: time.Now(),
	}
	return page, nil
}

func parseHtml(body io.Reader) (string, []string, error) {
	doc, err := html.Parse(body)
	if err != nil {
		return "", nil, err
	}

	var title string
	var links []string

	var f func(*html.Node)
	f = func(n *html.Node) {
		if n.Type == html.ElementNode {
			if n.Data == "title" && n.FirstChild != nil {
				title = n.FirstChild.Data
			}
			if n.Data == "a" {
				for _, attr := range n.Attr {
					if attr.Key == "href" && attr.Val != "" {
						links = append(links, attr.Val)
					}
				}
			}
		}

		for c := n.FirstChild; c != nil; c = c.NextSibling {
			f(c)
		}
	}

	f(doc)

	return strings.TrimSpace(title), links, nil

}
