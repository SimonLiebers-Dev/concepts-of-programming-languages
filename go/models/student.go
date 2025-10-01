package models

type Student struct {
	ID    int    `json:"id"`
	Name  string `json:"name"`
	Email string `json:"email"`
}

func (s *Student) GetID() int       { return s.ID }
func (s *Student) GetName() string  { return s.Name }
func (s *Student) GetEmail() string { return s.Email }
