package ui

import (
	"go-project/util"
)

type MenuScreen struct {
	BaseScreen
}

func NewMenuScreen() *MenuScreen {
	m := &MenuScreen{}
	m.Title = "MAIN MENU"
	m.Functions = map[string]string{
		"S": "Manage Students",
		"L": "Manage Lecturers",
		"C": "Manage Courses",
		"Q": "Quit",
	}
	return m
}

func (m *MenuScreen) Render() {
	util.ClearScreen()
	m.RenderHeader()
	m.RenderFunctions()
}

func (m *MenuScreen) HandleInput(input string) (Screen, bool) {
	switch input {
	case "S", "s":
		return NewStudentOverviewScreen(), false
	case "L", "l":
		return NewLecturerOverviewScreen(), false
	case "C", "c":
		return NewCourseOverviewScreen(), false
	case "Q", "q":
		return nil, true
	default:
		return nil, false
	}
}
