using Microsoft.Extensions.DependencyInjection;
using WebScraper.Cli.App;
using WebScraper.Cli.Util;

namespace WebScraper.Cli.Tests;

[TestFixture]
public class ProgramTests
{
    [Test]
    public async Task Program_ShouldInvoke_ApplicationRunAsync()
    {
        var fakeApp = new FakeApp();
        var provider = new ServiceCollection()
            .AddSingleton<IApplication>(_ => fakeApp)
            .BuildServiceProvider();

        await Program.RunAsync([], provider);

        Assert.That(fakeApp.RunCalled, Is.True);
    }

    [Test]
    public void CreateServiceProvider_ShouldReturnWorkingApplication()
    {
        // Arrange
        var provider = ServiceProviderUtils.CreateServiceProvider();

        // Act
        var app = provider.GetService<IApplication>();

        // Assert
        Assert.That(app, Is.Not.Null);
    }
}

internal class FakeApp : IApplication
{
    public bool RunCalled { get; private set; }

    public async Task RunAsync(CancellationToken ct = default)
    {
        RunCalled = true;
        await Task.CompletedTask;
    }
}