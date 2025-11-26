package main

import (
	"context"
	"go-scraper/app"
	"log"
	"time"
)

const (
	// AppTimeout defines the maximum execution time for the entire application
	AppTimeout = 10 * time.Minute
)

func main() {
	ctx, cancel := context.WithTimeout(context.Background(), AppTimeout)
	defer cancel()

	if err := app.Run(ctx); err != nil {
		log.Fatalf("Application error: %v", err)
	}
}
