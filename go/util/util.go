package util

import (
	"bufio"
	"fmt"
	"os"
	"strconv"
	"strings"

	"github.com/inancgumus/screen"
)

func ClearScreen() {
	screen.Clear()
	screen.MoveTopLeft()
}

// AskForInt prompts the user until a valid integer is entered
func AskForInt(prompt string) int {
	reader := bufio.NewReader(os.Stdin)
	for {
		fmt.Print(prompt)
		input, err := reader.ReadString('\n')
		if err != nil {
			fmt.Println("❌ Error reading input, try again")
			continue
		}

		input = strings.TrimSpace(input)
		if input == "" {
			fmt.Println("❌ Input cannot be empty")
			continue
		}

		value, err := strconv.Atoi(input)
		if err != nil {
			fmt.Println("❌ Invalid number, must be an integer")
			continue
		}

		return value
	}
}

// AskForString prompts until a non-empty string is entered
func AskForString(prompt string) string {
	reader := bufio.NewReader(os.Stdin)
	for {
		fmt.Print(prompt)
		input, err := reader.ReadString('\n')
		if err != nil {
			fmt.Println("❌ Error reading input, try again")
			continue
		}

		input = strings.TrimSpace(input)
		if input == "" {
			fmt.Println("❌ Input cannot be empty")
			continue
		}

		return input
	}
}

// AskForOptionalInt prompts for an integer, allows empty input to return 0
func AskForOptionalInt(prompt string) int {
	reader := bufio.NewReader(os.Stdin)
	for {
		fmt.Print(prompt)
		input, err := reader.ReadString('\n')
		if err != nil {
			fmt.Println("❌ Error reading input, try again")
			continue
		}

		input = strings.TrimSpace(input)
		if input == "" {
			return 0
		}

		value, err := strconv.Atoi(input)
		if err != nil {
			fmt.Println("❌ Invalid number, try again")
			continue
		}

		return value
	}
}
