package ui

import (
	"bufio"
	"fmt"
	"go-project/util"
	"os"
	"strings"
)

type MenuScreen struct {
	BaseScreen
}

func NewMenuScreen(manager *ScreenManager) *MenuScreen {
	s := &MenuScreen{}
	s.Title = "MAIN MENU"
	s.Functions = map[string]string{
		"S": "Manage Students",
		"L": "Manage Lecturers",
		"C": "Manage Courses",
		"Q": "Quit",
	}
	s.FunctionsOrder = []string{
		"S", "L", "C", "Q",
	}
	s.manager = manager
	return s
}

func (s *MenuScreen) Render() {
	util.ClearScreen()
	s.RenderHeader()
	s.RenderFunctionList()

	reader := bufio.NewReader(os.Stdin)
	for {
		input, err := reader.ReadString('\n')
		if err != nil {
			fmt.Println("❌ Error:", err)
		} else {
			input = strings.TrimSpace(input)
			functionCalled := s.TryHandleFunctionKey(input)
			if functionCalled {
				break
			}

			// Rerender screen if no valid function was pressed
			s.Render()
		}
	}
}

func (s *MenuScreen) TryHandleFunctionKey(input string) bool {
	switch input {
	case "S", "s":
		s.manager.PushScreen(NewStudentOverviewScreen(s.manager))
		return true
	case "L", "l":
		s.manager.PushScreen(NewLecturerOverviewScreen(s.manager))
		return true
	case "C", "c":
		s.manager.PushScreen(NewCourseOverviewScreen(s.manager))
		return true
	default:
		return false
	}
}
