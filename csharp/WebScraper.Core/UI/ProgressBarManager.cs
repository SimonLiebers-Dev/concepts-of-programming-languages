using System.Collections.Concurrent;
using Spectre.Console;
using WebScraper.Core.Extensions;

namespace WebScraper.Core.UI;

/// <summary>
/// Provides a managed, asynchronous wrapper around <see cref="Spectre.Console.Progress"/>
/// for rendering multiple progress bars concurrently in a CLI environment.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="ProgressBarManager"/> encapsulates Spectre.Console's progress rendering loop.
/// </para>
/// <para>
/// To use this class, call <see cref="StartRenderingAsync"/> before creating any tasks,
/// and ensure that <see cref="StopRendererAsync"/> or <see cref="DisposeAsync"/> is called
/// when scraping or other long-running operations complete.
/// </para>
/// </remarks>
internal class ProgressBarManager : IProgressBarManager
{
    private readonly ConcurrentBag<ProgressTask> _tasks = [];
    private readonly Progress _progress;
    private readonly CancellationTokenSource _cts = new();
    private Task? _renderTask;

    private readonly TaskCompletionSource<ProgressContext>? _ctxReady =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private ProgressContext? Context { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressBarManager"/> class.
    /// </summary>
    /// <remarks>
    /// Configures a <see cref="Spectre.Console.Progress"/> instance with columns for
    /// task descriptions, progress bars, percentage, elapsed time, and a spinner.
    /// The progress display does not clear automatically after completion, allowing
    /// users to review final results.
    /// </remarks>
    public ProgressBarManager()
    {
        _progress = AnsiConsole.Progress()
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(
                new TaskDescriptionColumn { Alignment = Justify.Left },
                new ProgressBarColumn() { Width = 25 },
                new PercentageColumn(),
                new ElapsedTimeColumn
                {
                    Style = Color.Aquamarine1
                },
                new SpinnerColumn());
    }

    /// <inheritdoc />
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

        // Wait until ProgressContext is available
        Context = await _ctxReady!.Task.ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ProgressTask CreateProgressBar(string url, int maxValue)
    {
        if (Context is null)
            throw new InvalidOperationException("Renderer not started. Call StartRendering() first.");

        var task = Context.AddTask(url.GetDescription("Fetching", "yellow"),
            autoStart: true, maxValue: maxValue);
        _tasks.Add(task);

        return task;
    }

    /// <inheritdoc />
    public async ValueTask StopRendererAsync()
    {
        await _cts.CancelAsync();

        // Mark all tasks complete to avoid console layout issues
        foreach (var task in _tasks)
            if (!task.IsFinished)
                task.StopTask();

        // Wait for Spectre to flush the final frame
        await Task.Delay(100);
    }

    /// <summary>
    /// Performs asynchronous cleanup by stopping the renderer and disposing resources.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous disposal process.</returns>
    public async ValueTask DisposeAsync()
    {
        await StopRendererAsync();
        _cts.Dispose();
    }
}