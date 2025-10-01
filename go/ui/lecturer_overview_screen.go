package ui

import (
	"go-project/util"
)

type LecturerOverviewScreen struct {
	BaseScreen
}

func NewLecturerOverviewScreen() *LecturerOverviewScreen {
	s := &LecturerOverviewScreen{}
	s.Title = "LECTURERS"
	s.Functions = map[string]string{
		"B": "Back",
	}
	return s
}

func (s *LecturerOverviewScreen) Render() {
	util.ClearScreen()
	s.RenderHeader()
	s.RenderFunctions()
}

func (s *LecturerOverviewScreen) HandleInput(input string) (Screen, bool) {
	switch input {
	case "B", "b":
		return nil, false
	default:
		return nil, false
	}
}
