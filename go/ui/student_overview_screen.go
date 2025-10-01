package ui

import (
	"go-project/util"
)

type StudentOverviewScreen struct {
	BaseScreen
}

func NewStudentOverviewScreen() *StudentOverviewScreen {
	s := &StudentOverviewScreen{}
	s.Title = "STUDENTS"
	s.Functions = map[string]string{
		"i": "Show Student Information",
		"B": "Back",
	}
	return s
}

func (s *StudentOverviewScreen) Render() {
	util.ClearScreen()
	s.RenderHeader()
	s.RenderFunctions()
}

func (s *StudentOverviewScreen) HandleInput(input string) (Screen, bool) {
	switch input {
	case "B", "b":
		return nil, false
	default:
		return nil, false
	}
}
