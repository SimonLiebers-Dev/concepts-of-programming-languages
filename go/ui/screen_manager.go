package ui

import "fmt"

type ScreenManager struct {
	screens []Screen
	running bool
}

func NewScreenManager() *ScreenManager {
	return &ScreenManager{
		screens: []Screen{},
		running: true,
	}
}

func (sm *ScreenManager) PushScreen(s Screen) {
	sm.screens = append(sm.screens, s)
}

func (sm *ScreenManager) GoBack() {
	if len(sm.screens) > 1 {
		sm.screens = sm.screens[:len(sm.screens)-1]
	} else {
		sm.Quit()
	}
}

func (sm *ScreenManager) Quit() {
	sm.running = false
}

func (sm *ScreenManager) CurrentScreen() Screen {
	if len(sm.screens) == 0 {
		return nil
	}
	return sm.screens[len(sm.screens)-1]
}

func (sm *ScreenManager) Run() {
	for sm.running && len(sm.screens) > 0 {
		current := sm.CurrentScreen()
		if current != nil {
			current.Render()
			fmt.Println("RENDER")
		}
	}
	fmt.Println("Exiting program.")
}
