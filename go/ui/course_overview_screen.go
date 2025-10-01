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

type CourseOverviewScreen struct {
	BaseScreen
}

func NewCourseOverviewScreen(manager *ScreenManager) *CourseOverviewScreen {
	c := &CourseOverviewScreen{}
	c.Title = "COURSES"
	c.Functions = map[string]string{
		"B": "Back",
		"C": "Create Course",
	}
	c.FunctionsOrder = []string{
		"C", "B",
	}
	c.manager = manager
	return c
}

func (s *CourseOverviewScreen) Render() {
	util.ClearScreen()
	s.RenderHeader()
	s.RenderFunctions()

	store := core.GetDataStore()
	courses := store.Courses
	if len(courses) == 0 {
		fmt.Println("No courses found.")
	} else {
		fmt.Println("List of Courses:")
		fmt.Println("----------------")
		for _, course := range courses {
			fmt.Printf("ID: %d | Name: %s | Lecturer: %d\n", course.ID, course.Name, course.LecturerID)
		}
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
				fmt.Println("❌ Invalid input, please enter a valid course id")
			} else {
				_, courseExists := store.GetCourseById(id)
				if courseExists {
					fmt.Printf("✅ Course with ID %d exists\n", id)
				} else {
					fmt.Printf("❌ No course found with ID %d\n", id)
				}
			}
		}
	}
}

func (s *CourseOverviewScreen) TryHandleFunctionKey(input string) bool {
	switch input {
	case "B", "b":
		s.manager.GoBack()
		return true
	case "C", "c":
		s.manager.PushScreen(NewCourseCreateScreen(s.manager))
		return true
	default:
		return false
	}
}
