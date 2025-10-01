package ui

type Screen interface {
	Render()
	HandleInput(input string) (next Screen, quit bool)
}
