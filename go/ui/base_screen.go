package ui

import "fmt"

type BaseScreen struct {
	Title          string
	Functions      map[string]string
	FunctionsOrder []string
	manager        *ScreenManager
}

func (b *BaseScreen) RenderHeader() {
	fmt.Println("===== " + b.Title + " =====")
}

func (b *BaseScreen) RenderFunctions() {
	line := ""
	for _, k := range b.FunctionsOrder {
		line += fmt.Sprintf("[%s: %s] ", k, b.Functions[k])
	}
	fmt.Println(line)
}

func (b *BaseScreen) RenderFunctionList() {
	for _, k := range b.FunctionsOrder {
		fmt.Printf("%s: %s \n", k, b.Functions[k])
	}
}

func (b *BaseScreen) Manager() *ScreenManager {
	return b.manager
}
