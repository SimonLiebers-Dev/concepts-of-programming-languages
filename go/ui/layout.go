package ui

import (
	"fmt"
	"strings"
)

// PrintHeader renders the app's ASCII header and metadata.
func PrintHeader() {
	const asciiHeader = `
__          __  _     _____                                
\ \        / / | |   / ____|                               
 \ \  /\  / /__| |__| (___   ___ _ __ __ _ _ __   ___ _ __ 
  \ \/  \/ / _ \ '_ \\___ \ / __| '__/ _' | '_ \ / _ \ '__|	
   \  /\  /  __/ |_) |___) | (__| | | (_| | |_) |  __/ |
    \/  \/ \___|_.__/_____/ \___|_|  \__,_| .__/ \___|_|
                                          | |
                                          |_|              `

	info := []string{
		"📦  Parallel Web Scraper (CLI Tool)",
		"👨‍💻  Developer: Simon Liebers",
		"🌐  GitHub: https://github.com/SimonLiebers-Dev/concepts-of-programming-languages",
		"🧠  Built with: Go 1.25",
	}

	// Print ASCII header
	fmt.Println(asciiHeader)

	// Print metadata
	for _, line := range info {
		fmt.Println(line)
	}
}

// PrintSeparator renders a text based separator
func PrintSeparator() {
	fmt.Println(strings.Repeat("─", 100))
}
