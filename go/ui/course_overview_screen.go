package ui

import (
	"go-project/util"
)

type CourseOverviewScreen struct {
	BaseScreen
}

func NewCourseOverviewScreen() *CourseOverviewScreen {
	s := &CourseOverviewScreen{}
	s.Title = "COURSES"
	s.Functions = map[string]string{
		"B": "Back",
	}
	return s
}

func (s *CourseOverviewScreen) Render() {
	util.ClearScreen()
	s.RenderHeader()
	s.RenderFunctions()
}

func (s *CourseOverviewScreen) HandleInput(input string) (Screen, bool) {
	switch input {
	case "B", "b":
		return nil, false
	default:
		return nil, false
	}
}
