package util_test

import (
	"errors"
	"fmt"
	"go-scraper/models"
	"go-scraper/util"
	"os"
	"testing"
)

// mockFileSystem implements util.FileSystem for testing.
type mockFileSystem struct {
	files map[string][]byte
	err   error
}

func newMockFS() *mockFileSystem {
	return &mockFileSystem{files: make(map[string][]byte)}
}

func (m *mockFileSystem) Stat(name string) (os.FileInfo, error) {
	if _, ok := m.files[name]; ok {
		return nil, nil
	}
	return nil, os.ErrNotExist
}

func (m *mockFileSystem) ReadFile(name string) ([]byte, error) {
	if m.err != nil {
		return nil, m.err
	}
	data, ok := m.files[name]
	if !ok {
		return nil, os.ErrNotExist
	}
	return data, nil
}

func (m *mockFileSystem) WriteFile(name string, data []byte, _ os.FileMode) error {
	if m.err != nil {
		return m.err
	}
	m.files[name] = data
	return nil
}

func (m *mockFileSystem) MakeDir(_ string) error {
	return nil
}

// fakeTimeProvider returns a fixed timestamp for reproducible tests.
type fakeTimeProvider struct{}

func (fakeTimeProvider) NowUnixMilli() int64 { return 1234567890 }

func TestGetURLsFromFile_CreateIfMissing(t *testing.T) {
	fs := newMockFS()
	urls, err := util.GetURLsFromFile(fs, "urls.json")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if len(urls) != 0 {
		t.Errorf("expected 0 URLs, got %d", len(urls))
	}
	if _, ok := fs.files["urls.json"]; !ok {
		t.Error("expected file to be created")
	}
}

func TestGetURLsFromFile_ReadExisting(t *testing.T) {
	fs := newMockFS()
	fs.files["urls.json"] = []byte(`["https://a.com", "https://b.com"]`)

	urls, err := util.GetURLsFromFile(fs, "urls.json")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if len(urls) != 2 {
		t.Errorf("expected 2 URLs, got %d", len(urls))
	}
}

func TestGetURLsFromFile_InvalidJSON(t *testing.T) {
	fs := newMockFS()
	fs.files["urls.json"] = []byte(`{not valid}`)
	_, err := util.GetURLsFromFile(fs, "urls.json")
	if err == nil {
		t.Fatal("expected error for invalid JSON, got nil")
	}
}

func TestSaveResultsToFile_Success(t *testing.T) {
	fs := newMockFS()
	tp := fakeTimeProvider{}
	pages := []*models.Page{
		{URL: "https://a.com", Title: "A"},
		{URL: "https://b.com", Title: "B"},
	}

	filename, err := util.SaveResultsToFile(fs, tp, "output", pages)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}

	expected := fmt.Sprintf("output/scrape-results-%d.json", tp.NowUnixMilli())
	if filename != expected {
		t.Errorf("expected filename %s, got %s", expected, filename)
	}
	if _, ok := fs.files[filename]; !ok {
		t.Errorf("expected file %s to be written", filename)
	}
}

func TestSaveResultsToFile_EmptyPages(t *testing.T) {
	fs := newMockFS()
	tp := fakeTimeProvider{}
	_, err := util.SaveResultsToFile(fs, tp, "output", []*models.Page{})
	if err == nil {
		t.Fatal("expected error for empty pages, got nil")
	}
}

func TestSaveResultsToFile_WriteError(t *testing.T) {
	fs := newMockFS()
	fs.err = errors.New("write failure")
	tp := fakeTimeProvider{}
	pages := []*models.Page{{URL: "https://a.com"}}

	_, err := util.SaveResultsToFile(fs, tp, "output", pages)
	if err == nil {
		t.Fatal("expected write error, got nil")
	}
}
