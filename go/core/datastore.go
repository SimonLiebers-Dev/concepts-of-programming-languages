package core

import (
	"encoding/json"
	"errors"
	"fmt"
	"go-project/models"
	"os"
	"sort"
	"sync"
)

type DataStore struct {
	Students  map[int]*models.Student
	Lecturers map[int]*models.Lecturer
	Courses   map[int]*models.Course

	mu sync.Mutex
}

var store *DataStore

func GetDataStore() *DataStore {
	if store == nil {
		store = &DataStore{
			Students:  make(map[int]*models.Student),
			Lecturers: make(map[int]*models.Lecturer),
			Courses:   make(map[int]*models.Course),
		}
	}
	return store
}

func (ds *DataStore) Load() error {
	// Check if file exists
	_, err := os.Stat(FilePath)
	if err != nil {
		if errors.Is(err, os.ErrNotExist) {
			// initialize empty datastore if no file yet
			ds.Students = make(map[int]*models.Student)
			ds.Lecturers = make(map[int]*models.Lecturer)
			ds.Courses = make(map[int]*models.Course)
			return nil
		}
		return err // some other error (e.g. permission issue)
	}

	// File exists → read contents
	bytes, err := os.ReadFile(FilePath)
	if err != nil {
		return err
	}

	// Unmarshal into datastore
	if err := json.Unmarshal(bytes, ds); err != nil {
		return err
	}

	// Ensure maps are not nil, in case JSON had empty objects
	if ds.Students == nil {
		ds.Students = make(map[int]*models.Student)
	}
	if ds.Lecturers == nil {
		ds.Lecturers = make(map[int]*models.Lecturer)
	}
	if ds.Courses == nil {
		ds.Courses = make(map[int]*models.Course)
	}

	return nil
}

func (ds *DataStore) Save() error {
	bytes, err := json.MarshalIndent(ds, "", "  ")
	if err != nil {
		return err
	}
	return os.WriteFile(FilePath, bytes, 0644)
}

func (ds *DataStore) AddStudent(s *models.Student) error {
	ds.mu.Lock()
	defer ds.mu.Unlock()

	if _, exists := ds.Students[s.ID]; exists {
		return fmt.Errorf("student with ID %d already exists", s.ID)
	}

	ds.Students[s.ID] = s
	return nil
}

func (ds *DataStore) DeleteStudent(id int) {
	ds.mu.Lock()
	defer ds.mu.Unlock()
	delete(ds.Students, id)
}

func (ds *DataStore) GetStudentById(id int) (*models.Student, bool) {
	ds.mu.Lock()
	defer ds.mu.Unlock()

	student, ok := ds.Students[id]
	return student, ok
}

func (ds *DataStore) GetStudentsSorted() []*models.Student {
	ds.mu.Lock()
	defer ds.mu.Unlock()

	students := make([]*models.Student, 0, len(ds.Students))
	for _, s := range ds.Students {
		students = append(students, s)
	}

	sort.Slice(students, func(i, j int) bool {
		return students[i].ID < students[j].ID
	})

	return students
}

func (ds *DataStore) AddCourse(c *models.Course) error {
	ds.mu.Lock()
	defer ds.mu.Unlock()

	if _, exists := ds.Courses[c.ID]; exists {
		return fmt.Errorf("course with ID %d already exists", c.ID)
	}

	ds.Courses[c.ID] = c
	return nil
}

func (ds *DataStore) DeleteCourse(id int) {
	ds.mu.Lock()
	defer ds.mu.Unlock()
	delete(ds.Courses, id)
}

func (ds *DataStore) GetCourseById(id int) (*models.Course, bool) {
	ds.mu.Lock()
	defer ds.mu.Unlock()

	course, ok := ds.Courses[id]
	return course, ok
}

func (ds *DataStore) GetCoursesByStudentId(studentID int) []*models.Course {
	ds.mu.Lock()
	defer ds.mu.Unlock()

	var courses []*models.Course
	for _, course := range ds.Courses {
		for _, id := range course.StudentIDs {
			if id == studentID {
				courses = append(courses, course)
				break
			}
		}
	}
	return courses
}

func (ds *DataStore) GetCoursesByLecturerId(lecturerID int) []*models.Course {
	ds.mu.Lock()
	defer ds.mu.Unlock()

	var courses []*models.Course
	for _, course := range ds.Courses {
		if course.LecturerID == lecturerID {
			courses = append(courses, course)
		}
	}
	return courses
}

func (ds *DataStore) AddLecturer(l *models.Lecturer) error {
	ds.mu.Lock()
	defer ds.mu.Unlock()

	if _, exists := ds.Courses[l.ID]; exists {
		return fmt.Errorf("course with ID %d already exists", l.ID)
	}

	ds.Lecturers[l.ID] = l
	return nil
}

func (ds *DataStore) DeleteLecturer(id int) {
	ds.mu.Lock()
	defer ds.mu.Unlock()
	delete(ds.Lecturers, id)
}

func (ds *DataStore) GetLecturerById(id int) (*models.Lecturer, bool) {
	ds.mu.Lock()
	defer ds.mu.Unlock()

	lecturer, ok := ds.Lecturers[id]
	return lecturer, ok
}
