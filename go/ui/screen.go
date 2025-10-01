package ui

type Screen interface {
	Render()
	TryHandleFunctionKey(input string) bool
}
