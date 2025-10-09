package models_test

import (
	"go-scraper/models"
	"testing"
)

func TestPage_HasError(t *testing.T) {
	tests := []struct {
		name     string
		page     *models.Page
		expected bool
	}{
		{"NoError", &models.Page{Error: ""}, false},
		{"WithError", &models.Page{Error: "network failed"}, true},
		{"NilPage", nil, false},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := tt.page.HasError(); got != tt.expected {
				t.Errorf("HasError() = %v, expected %v", got, tt.expected)
			}
		})
	}
}

func TestPage_Success(t *testing.T) {
	tests := []struct {
		name     string
		page     *models.Page
		expected bool
	}{
		{"Success", &models.Page{Error: ""}, true},
		{"Failure", &models.Page{Error: "timeout"}, false},
		{"NilPage", nil, false},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := tt.page.Success(); got != tt.expected {
				t.Errorf("Success() = %v, expected %v", got, tt.expected)
			}
		})
	}
}
