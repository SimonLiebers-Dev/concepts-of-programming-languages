package ui

import (
	"bufio"
	"fmt"
	"go-project/core"
	"go-project/util"
	"os"
	"strconv"
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

	idEntered := false
	for !idEntered {
		id, ok := s.readInt("Enter unique course id: ")
		if ok {
			_, found := s.dataStore.GetCourseById(id)
			if found {
				fmt.Println("❌ Course with this id already exists, try again")
				continue
			}
			idEntered = true
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

func (s *CourseCreateScreen) readString(prompt string) (string, bool) {
	reader := bufio.NewReader(os.Stdin)
	for {
		fmt.Println(prompt)
		input, err := reader.ReadString('\n')
		if err != nil {
			fmt.Println("❌ Error:", err)
			continue
		}

		input = strings.TrimSpace(input)

		// Check if input was a function key
		if s.TryHandleFunctionKey(input) {
			return "", false // signal: function handled, no normal input
		}

		return input, true // signal: valid input entered
	}
}

func (s *CourseCreateScreen) readInt(prompt string) (int, bool) {
	reader := bufio.NewReader(os.Stdin)
	for {
		fmt.Println(prompt)
		input, err := reader.ReadString('\n')
		if err != nil {
			fmt.Println("❌ Error reading input:", err)
			continue
		}

		input = strings.TrimSpace(input)

		// Handle function keys (like "B" for back)
		if s.TryHandleFunctionKey(input) {
			return 0, false // signal: function handled, not a valid int
		}

		// Convert string → int
		val, convErr := strconv.Atoi(input)
		if convErr != nil {
			fmt.Println("❌ Invalid number, try again")
			continue
		}

		return val, true // success
	}
}
