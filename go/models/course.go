package models

type Course struct {
	ID         int    `json:"id"`
	Name       string `json:"name"`
	LecturerID int    `json:"lecturer_id"`
	StudentIDs []int  `json:"student_ids"`
}
