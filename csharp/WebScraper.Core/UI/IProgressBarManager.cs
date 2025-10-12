using Spectre.Console;

namespace WebScraper.Core.UI;

public interface IProgressBarManager : IAsyncDisposable
{
    Task StartRenderingAsync();
    ProgressTask NewTracker(string label, double total = 100);
    void Increment(string label, double value);
    ValueTask StopRendererAsync();
}