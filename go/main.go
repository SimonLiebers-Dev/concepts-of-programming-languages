package main

import (
	"fmt"
	"go-project/core"
	"go-project/ui"
)

func main() {
	// Initialize data store
	store := core.GetDataStore()

	// Load data from file
	loadErr := store.Load()

	// Print error if data cannot be loaded
	if loadErr != nil {
		fmt.Println("⚠️ Could not load data:", loadErr) // Display error
		return
	}

	// Initialize new screen manager
	manager := ui.NewScreenManager()

	// Pass manager to screens in constructor
	manager.PushScreen(ui.NewMenuScreen(manager))

	manager.Run()

	saveErr := store.Save() // Save data to json file
	if saveErr != nil {
		fmt.Println("⚠️ Could not save data:", saveErr) // Display error
	}
}
