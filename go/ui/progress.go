package ui

import (
	"time"

	"github.com/jedib0t/go-pretty/v6/progress"
)

// ProgressBarManager manages multiple progress bars.
type ProgressBarManager struct {
	writer   progress.Writer
	trackers []*progress.Tracker
}

// NewProgressBarManager sets up and returns a new manager.
func NewProgressBarManager(numExpected int) *ProgressBarManager {
	writer := progress.NewWriter()
	writer.SetAutoStop(false)
	writer.SetTrackerLength(25)
	writer.SetMessageWidth(76)
	writer.SetTrackerPosition(progress.PositionRight)
	writer.SetStyle(progress.StyleDefault)
	writer.SetNumTrackersExpected(numExpected)
	writer.SetUpdateFrequency(100 * time.Millisecond)

	manager := &ProgressBarManager{
		writer:   writer,
		trackers: []*progress.Tracker{},
	}

	go manager.writer.Render()
	return manager
}

// NewTracker creates and registers a new tracker.
func (pbm *ProgressBarManager) NewTracker(label string, total int64) *progress.Tracker {
	tracker := &progress.Tracker{
		Message: label,
		Total:   total,
	}
	pbm.writer.AppendTracker(tracker)
	pbm.trackers = append(pbm.trackers, tracker)
	return tracker
}

// StopRenderer manually stops the progress writer rendering.
func (pbm *ProgressBarManager) StopRenderer() {
	pbm.writer.Stop()

	// Prevent formatting errors in console
	for pbm.writer.IsRenderInProgress() {
		time.Sleep(50 * time.Millisecond)
	}
}
