package main

import (
	"context"
	"go-scraper/app"
	"log"
	"time"
)

func main() {
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Minute)
	defer cancel()

	if err := app.Run(ctx); err != nil {
		log.Fatalf("❌ Application error: %v", err)
	}
}
