package ui

import (
	"bufio"
	"fmt"
	"go-project/core"
	"go-project/util"
	"os"
	"strconv"
	"strings"

	"github.com/jedib0t/go-pretty/v6/table"
)

type LecturerOverviewScreen struct {
	BaseScreen
}

func NewLecturerOverviewScreen(manager *ScreenManager) *LecturerOverviewScreen {
	s := &LecturerOverviewScreen{}
	s.Title = "LECTURERS"
	s.Functions = map[string]string{
		"B": "Back",
	}
	s.FunctionsOrder = []string{
		"B",
	}
	s.manager = manager
	return s
}

func (s *LecturerOverviewScreen) Render() {
	util.ClearScreen()
	s.RenderHeader()
	s.RenderFunctions()

	store := core.GetDataStore()
	lecturers := store.Lecturers
	if len(lecturers) == 0 {
		fmt.Println("No lecturers found.")
	} else {
		t := table.NewWriter()
		t.SetOutputMirror(os.Stdout)
		t.AppendHeader(table.Row{"#", "Name", "Email", "Number of courses"})
		for _, lecturer := range lecturers {
			lecturerCourses := store.GetCoursesByLecturerId(lecturer.ID)
			t.AppendRow(table.Row{lecturer.ID, lecturer.Name, lecturer.Email, len(lecturerCourses)})
		}
		t.Render()
	}

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

			id, parseErr := strconv.Atoi(input)

			if parseErr != nil {
				fmt.Println("❌ Invalid input, please enter a valid lecturer id")
				// handle invalid input, e.g., continue loop
			} else {
				// Check if student exists in datastore
				_, lecturerExists := store.GetLecturerById(id)
				if lecturerExists {
					fmt.Printf("✅ Lecturer with ID %d exists\n", id)
				} else {
					fmt.Printf("❌ No lecturer found with ID %d\n", id)
				}
			}
		}
	}
}

func (s *LecturerOverviewScreen) TryHandleFunctionKey(input string) bool {
	switch input {
	case "B", "b":
		s.manager.GoBack()
		return true
	default:
		return false
	}
}
