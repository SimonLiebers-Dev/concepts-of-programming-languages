package ui

import (
	"bufio"
	"fmt"
	"go-project/core"
	"go-project/util"
	"os"
	"strings"
)

type CourseCreateScreen struct {
	BaseScreen
	dataStore *core.DataStore
}

func NewCourseCreateScreen(manager *ScreenManager) *CourseCreateScreen {
	s := &CourseCreateScreen{}
	s.Title = "CREATE COURSE"
	s.Functions = map[string]string{
		"B": "Back",
	}
	s.FunctionsOrder = []string{
		"B",
	}
	s.dataStore = core.GetDataStore()
	s.manager = manager
	return s
}

func (s *CourseCreateScreen) Render() {
	util.ClearScreen()
	s.RenderHeader()
	s.RenderFunctions()

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
			
			// TODO: Input data to create course
		}
	}
}

func (s *CourseCreateScreen) TryHandleFunctionKey(input string) bool {
	switch input {
	case "B", "b":
		s.manager.GoBack()
		return true
	default:
		return false
	}
}

func (s *CourseCreateScreen) AskForCourseName() string {
	return util.AskForString("Enter course name: ")
}
