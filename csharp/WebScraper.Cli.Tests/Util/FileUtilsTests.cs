using System.Text.Json;
using WebScraper.Cli.Util;
using WebScraper.Core.Models;

namespace WebScraper.Cli.Tests.Util;

[TestFixture]
public class FileUtilsTests
{
    private string _tempDir = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    [Test]
    public async Task GetUrlsFromFileAsync_ShouldCreateEmptyFile_WhenNotExists()
    {
        // Arrange
        var path = Path.Combine(_tempDir, "urls.json");

        // Act
        var urls = await FileUtils.GetUrlsFromFileAsync(path);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(path), Is.True);
            Assert.That(urls, Is.Empty);
        });

        var fileContent = await File.ReadAllTextAsync(path);
        Assert.That(fileContent.Trim(), Is.EqualTo("[]"));
    }

    [Test]
    public async Task GetUrlsFromFileAsync_ShouldReadUrlsFromValidJson()
    {
        // Arrange
        var path = Path.Combine(_tempDir, "urls.json");
        var urls = new List<string> { "https://example.com", "https://openai.com" };
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(urls));

        // Act
        var result = await FileUtils.GetUrlsFromFileAsync(path);

        // Assert
        Assert.That(result, Is.EquivalentTo(urls));
    }

    [Test]
    public void GetUrlsFromFileAsync_ShouldThrow_WhenJsonIsInvalid()
    {
        // Arrange
        var path = Path.Combine(_tempDir, "urls.json");
        File.WriteAllText(path, "{ invalid json");

        // Act + Assert
        var ex = Assert.ThrowsAsync<JsonException>(async () =>
            await FileUtils.GetUrlsFromFileAsync(path));

        Assert.That(ex!.Message, Does.Contain("Invalid JSON"));
    }

    [Test]
    public void SaveResultsToFileAsync_ShouldThrow_WhenPagesEmpty()
    {
        // Arrange
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await FileUtils.SaveResultsToFileAsync([], _tempDir, timestamp));

        // Assert
        Assert.That(ex!.Message, Does.Contain("No pages to save"));
    }

    [Test]
    public async Task SaveResultsToFileAsync_ShouldSaveJsonFileSuccessfully()
    {
        // Arrange
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1234567890);
        var page = Page.SuccessPage("https://example.com", "Title", ["link1"], ["img1"], timestamp);
        var pages = new[] { page };

        // Act
        var filePath = await FileUtils.SaveResultsToFileAsync(pages, _tempDir, timestamp);

        // Assert
        Assert.That(File.Exists(filePath), Is.True);

        var json = await File.ReadAllTextAsync(filePath);
        var deserialized = JsonSerializer.Deserialize<List<Page>>(json);

        Assert.That(deserialized, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized!.Count, Is.EqualTo(1));
            Assert.That(deserialized[0].Url, Is.EqualTo(page.Url));
        });
    }

    [Test]
    public async Task SaveResultsToFileAsync_ShouldCreateOutputDirectoryIfMissing()
    {
        // Arrange
        var missingDir = Path.Combine(_tempDir, "nested/output");
        var page = Page.SuccessPage("https://example.com", "Title");
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        var filePath = await FileUtils.SaveResultsToFileAsync([page], missingDir, timestamp);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(Directory.Exists(missingDir), Is.True);
            Assert.That(File.Exists(filePath), Is.True);
        });
    }

    [Test]
    public async Task SaveResultsToFileAsync_ShouldUseTimestampInFilename()
    {
        // Arrange
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(999999999);
        var page = Page.SuccessPage("https://example.com", "Title");
        var pages = new[] { page };

        // Act
        var filePath = await FileUtils.SaveResultsToFileAsync(pages, _tempDir, timestamp);

        // Assert
        var filename = Path.GetFileName(filePath);
        Assert.That(filename, Does.Contain("999999999"));
    }
}