package models

type Lecturer struct {
	ID    int    `json:"id"`
	Name  string `json:"name"`
	Email string `json:"email"`
}

func (l *Lecturer) GetID() int       { return l.ID }
func (l *Lecturer) GetName() string  { return l.Name }
func (l *Lecturer) GetEmail() string { return l.Email }
