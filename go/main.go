package main

import (
	"bufio"
	"fmt"
	"go-project/ui"
	"os"
	"strings"
)

func main() {
	reader := bufio.NewReader(os.Stdin)

	screenStack := []ui.Screen{ui.NewMenuScreen()}

	for len(screenStack) > 0 {
		current := screenStack[len(screenStack)-1]
		current.Render()

		input, _ := reader.ReadString('\n')
		input = strings.TrimSpace(input)

		nextScreen, quit := current.HandleInput(input)
		if quit {
			break
		}

		if nextScreen != nil {
			screenStack = append(screenStack, nextScreen)
		} else if input == "B" || input == "b" {
			screenStack = screenStack[:len(screenStack)-1]
		}
	}

	fmt.Println("Bye!")
}
