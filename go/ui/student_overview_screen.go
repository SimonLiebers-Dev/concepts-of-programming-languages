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

type StudentOverviewScreen struct {
	BaseScreen
}

func NewStudentOverviewScreen(manager *ScreenManager) *StudentOverviewScreen {
	s := &StudentOverviewScreen{}
	s.Title = "STUDENTS"
	s.Functions = map[string]string{
		"i": "Show Student Information",
		"B": "Back",
	}
	s.FunctionsOrder = []string{
		"i", "B",
	}
	s.manager = manager
	return s
}

func (s *StudentOverviewScreen) Render() {
	util.ClearScreen()
	s.RenderHeader()
	s.RenderFunctions()

	store := core.GetDataStore()
	students := store.Students
	if len(students) == 0 {
		fmt.Println("No students found.")
	} else {
		t := table.NewWriter()
		t.SetOutputMirror(os.Stdout)
		t.AppendHeader(table.Row{"#", "Name", "Email", "Number of courses"})
		for _, student := range students {
			studentCourses := store.GetCoursesByStudentId(student.ID)
			t.AppendRow(table.Row{student.ID, student.Name, student.Email, len(studentCourses)})
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
				fmt.Println("❌ Invalid input, please enter a valid student id")
				// handle invalid input, e.g., continue loop
			} else {
				// Check if student exists in datastore
				_, studentExists := store.GetStudentById(id)
				if studentExists {
					fmt.Printf("✅ Student with ID %d exists\n", id)
				} else {
					fmt.Printf("❌ No student found with ID %d\n", id)
				}
			}
		}
	}
}

func (s *StudentOverviewScreen) TryHandleFunctionKey(input string) bool {
	switch input {
	case "B", "b":
		s.manager.GoBack()
		return true
	default:
		return false
	}
}
