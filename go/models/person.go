package models

type Person interface {
	GetID() int
	GetName() string
	GetEmail() string
}
