package ui

import "fmt"

type BaseScreen struct {
	Title     string
	Functions map[string]string
}

func (b *BaseScreen) RenderHeader() {
	fmt.Println("===== " + b.Title + " =====")
}

func (b *BaseScreen) RenderFunctions() {
	line := ""
	for key, desc := range b.Functions {
		line += fmt.Sprintf("[%s: %s] ", key, desc)
	}
	fmt.Println(line)
}
