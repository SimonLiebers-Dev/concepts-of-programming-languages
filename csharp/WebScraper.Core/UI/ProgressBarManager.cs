using System.Collections.Concurrent;
using Spectre.Console;

namespace WebScraper.Core.UI;

/// <summary>
/// Manages a set of live progress bars using Spectre.Console.
/// Provides a simple API similar to the Go pretty/progress manager.
/// </summary>
internal class ProgressBarManager : IProgressBarManager
{
    private readonly ConcurrentDictionary<string, ProgressTask> _tasks = new();
    private readonly Progress _progress;
    private readonly CancellationTokenSource _cts = new();
    private Task? _renderTask;
    private readonly TaskCompletionSource<ProgressContext>? _ctxReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private ProgressContext? Context { get; set; }

    public ProgressBarManager()
    {
        _progress = AnsiConsole.Progress()
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(
                new TaskDescriptionColumn { Alignment = Justify.Left },
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn(),
                new RemainingTimeColumn());
    }

    /// <summary>
    /// Starts the live renderer and waits until the context is ready.
    /// </summary>
    public async Task StartRenderingAsync()
    {
        if (_renderTask is not null)
            return;

        _renderTask = Task.Run(async () =>
        {
            await _progress.StartAsync(async ctx =>
            {
                Context = ctx;
                _ctxReady?.TrySetResult(ctx);

                try
                {
                    await Task.Delay(Timeout.Infinite, _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    // expected on StopRendererAsync
                }
            });
        });

        // Wait until Spectre's ProgressContext is available
        Context = await _ctxReady!.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Creates and registers a new progress tracker.
    /// </summary>
    public ProgressTask NewTracker(string label, double total = 100)
    {
        if (Context is null)
            throw new InvalidOperationException("Renderer not started. Call StartRendering() first.");

        var task = Context.AddTask(label, autoStart: true, maxValue: total);
        _tasks[label] = task;
        return task;
    }

    /// <summary>
    /// Updates a tracker by label.
    /// </summary>
    public void Increment(string label, double value)
    {
        if (_tasks.TryGetValue(label, out var task))
        {
            task.Increment(value);
        }
    }

    /// <summary>
    /// Completes and stops rendering.
    /// </summary>
    public async ValueTask StopRendererAsync()
    {
        await _cts.CancelAsync();

        // Mark all tasks complete to avoid console layout issues
        foreach (var task in _tasks.Values)
            if (!task.IsFinished)
                task.StopTask();
        
        // Wait briefly for Spectre to flush the final frame
        await Task.Delay(100);
    }

    public async ValueTask DisposeAsync()
    {
        await StopRendererAsync();
        _cts.Dispose();
    }
}