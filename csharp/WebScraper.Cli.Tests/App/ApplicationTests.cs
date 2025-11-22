using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using WebScraper.Cli.App;
using WebScraper.Core.Fetcher;
using WebScraper.Core.Models;
using WebScraper.Core.Scraping;

namespace WebScraper.Cli.Tests.App;

[TestFixture]
public class ApplicationTests
{
    private Mock<IConfiguration> _configMock = null!;
    private Mock<IHtmlFetcher> _fetcherMock = null!;
    private Mock<IScrapeRunner> _runnerMock = null!;
    private StringBuilder _consoleOutput = null!;
    private StringWriter _writer = null!;
    private TextReader _originalIn = null!;
    private TextWriter _originalOut = null!;

    [SetUp]
    public void SetUp()
    {
        _configMock = new Mock<IConfiguration>();
        _fetcherMock = new Mock<IHtmlFetcher>();
        _runnerMock = new Mock<IScrapeRunner>();

        _consoleOutput = new StringBuilder();
        _writer = new StringWriter(_consoleOutput);
        _originalOut = Console.Out;
        _originalIn = Console.In;
        Console.SetOut(_writer);
    }

    [Test]
    public async Task RunAsync_ShouldExit_WhenConfigInvalid()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var app = new Application(configuration, _fetcherMock.Object, _runnerMock.Object);

        // Act
        await app.RunAsync();

        // Assert
        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain("Missing required configuration section"));
    }

    [Test]
    public async Task RunAsync_ShouldExit_WhenNoUrlsConfigured()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, "[]");

        var configRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Scraper:UrlsFile"] = tempFile,
                ["Scraper:ResultsDirectory"] = Path.GetTempPath(),
                ["Scraper:Concurrency"] = "2",
                ["Scraper:HttpTimeoutSeconds"] = "5",
                ["Scraper:UserAgent"] = "UserAgent"
            })
            .Build();

        var app = new Application(configRoot, _fetcherMock.Object, _runnerMock.Object);

        // Act
        await app.RunAsync();

        // Assert
        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain("No URLs configured"));
    }

    [Test]
    public async Task RunAsync_ShouldRunSequential_WhenUserChooses1()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, "[\"https://example.com\"]");

        var configRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Scraper:UrlsFile"] = tempFile,
                ["Scraper:ResultsDirectory"] = Path.GetTempPath(),
                ["Scraper:Concurrency"] = "2",
                ["Scraper:HttpTimeoutSeconds"] = "5",
                ["Scraper:UserAgent"] = "UserAgent"
            })
            .Build();

        _runnerMock.Setup(r => r.RunSequentialAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([Page.SuccessPage("https://example.com", "Example")]);

        Console.SetIn(new StringReader("1\nn\n")); // Choose sequential, then don't save
        var app = new Application(configRoot, _fetcherMock.Object, _runnerMock.Object);

        // Act
        await app.RunAsync();

        // Assert
        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain("Running sequential scraper"));
        Assert.That(output, Does.Contain("1 successful"));
    }

    [Test]
    public async Task RunAsync_ShouldRunParallel_WhenUserChooses2()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, "[\"http://example.com\"]");

        var configRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Scraper:UrlsFile"] = tempFile,
                ["Scraper:ResultsDirectory"] = Path.GetTempPath(),
                ["Scraper:Concurrency"] = "4",
                ["Scraper:HttpTimeoutSeconds"] = "10",
                ["Scraper:UserAgent"] = "UserAgent"
            })
            .Build();

        _runnerMock.Setup(r => r.RunParallelAsync(It.IsAny<IReadOnlyList<string>>(), 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync([Page.SuccessPage("https://example.com", "Example")]);

        Console.SetIn(new StringReader("2\ny\n")); // Choose parallel, then save
        var app = new Application(configRoot, _fetcherMock.Object, _runnerMock.Object);

        // Act
        await app.RunAsync();

        // Assert
        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain("Running parallel scraper"));
        Assert.That(output, Does.Contain("1 successful"));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenDependenciesNull()
    {
        // Act
        var ex1 = Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new Application(null!, _fetcherMock.Object, _runnerMock.Object);
        });
        var ex2 = Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new Application(_configMock.Object, _fetcherMock.Object, null!);
        });
        var ex3 = Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new Application(_configMock.Object, null!, _runnerMock.Object);
        });

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex1!.ParamName, Is.EqualTo("configuration"));
            Assert.That(ex2!.ParamName, Is.EqualTo("runner"));
            Assert.That(ex3!.ParamName, Is.EqualTo("fetcher"));
        }
    }

    [TearDown]
    public void TearDown()
    {
        _writer.Dispose();
        Console.SetOut(_originalOut);
        Console.SetIn(_originalIn);
    }
}