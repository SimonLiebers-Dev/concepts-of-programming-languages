package ui

import (
	"time"

	"github.com/jedib0t/go-pretty/v6/progress"
)

const (
	// ProgressBarLength defines the character width of each progress bar
	ProgressBarLength = 25
	// ProgressMessageWidth defines the maximum width for progress bar messages (URL labels)
	ProgressMessageWidth = 76
	// ProgressUpdateFrequency defines how often progress bars refresh
	ProgressUpdateFrequency = 100 * time.Millisecond
	// ProgressStopCheckInterval defines how long to wait when checking if rendering stopped
	ProgressStopCheckInterval = 50 * time.Millisecond
)

// ProgressBarManager manages multiple progress bars for tracking concurrent operations.
// It uses the go-pretty library to render live progress updates in the console.
type ProgressBarManager struct {
	writer   progress.Writer        // Underlying progress writer from go-pretty
	trackers []*progress.Tracker    // Slice of all progress trackers being managed
}

// NewProgressBarManager creates and initializes a new progress bar manager.
// The numExpected parameter indicates how many progress bars will be added.
// The manager starts rendering immediately in a background goroutine.
func NewProgressBarManager(numExpected int) *ProgressBarManager {
	writer := progress.NewWriter()
	writer.SetAutoStop(false)
	writer.SetTrackerLength(ProgressBarLength)
	writer.SetMessageWidth(ProgressMessageWidth)
	writer.SetTrackerPosition(progress.PositionRight)
	writer.SetStyle(progress.StyleDefault)
	writer.SetNumTrackersExpected(numExpected)
	writer.SetUpdateFrequency(ProgressUpdateFrequency)

	manager := &ProgressBarManager{
		writer:   writer,
		trackers: []*progress.Tracker{},
	}

	go manager.writer.Render()
	return manager
}

// NewTracker creates and registers a new progress tracker with the given label and total steps.
// The tracker is automatically added to the progress bar display.
// Returns the tracker which can be used to update progress (Increment, MarkAsErrored, etc.).
func (pbm *ProgressBarManager) NewTracker(label string, total int64) *progress.Tracker {
	tracker := &progress.Tracker{
		Message: label,
		Total:   total,
	}
	pbm.writer.AppendTracker(tracker)
	pbm.trackers = append(pbm.trackers, tracker)
	return tracker
}

// StopRenderer stops the progress bar rendering and waits for it to fully stop.
// This should be called when all progress tracking is complete to clean up the display.
// It blocks until rendering has completely stopped to prevent console formatting issues.
func (pbm *ProgressBarManager) StopRenderer() {
	pbm.writer.Stop()

	// Wait for rendering to fully stop to prevent console formatting errors
	for pbm.writer.IsRenderInProgress() {
		time.Sleep(ProgressStopCheckInterval)
	}
}
