using WebScraper.Core.Models;

namespace WebScraper.Tests.Models;

[TestFixture]
public class PageTests
{
    [TestCase("NoError", "", ExpectedResult = false)]
    [TestCase("WithError", "network failed", ExpectedResult = true)]
    public bool HasError_ShouldReflectErrorState(string _, string error)
    {
        var page = new Page("https://example.org", "t", [], DateTimeOffset.UtcNow, error);
        return page.HasError;
    }

    [TestCase("Success", "", ExpectedResult = true)]
    [TestCase("Failure", "timeout", ExpectedResult = false)]
    public bool Success_ShouldReflectErrorState(string _, string error)
    {
        var page = new Page("https://example.org", "t", [], DateTimeOffset.UtcNow, error);
        return page.Success;
    }
    
    [Test]
    public void SuccessPage_ShouldInitializeCorrectly()
    {
        // Arrange
        const string url = "https://example.org";
        const string title = "Example";
        var links = new[] { "https://a.com", "https://b.com" };

        // Act
        var page = Page.SuccessPage(url, title, links);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(page.Url, Is.EqualTo(url));
            Assert.That(page.Title, Is.EqualTo(title));
            Assert.That(page.Links, Is.EquivalentTo(links));
            Assert.That(page.Error, Is.Null);
            Assert.That(page.HasError, Is.False);
            Assert.That(page.Success, Is.True);
            Assert.That(page.Timestamp, Is.Not.EqualTo(default(DateTimeOffset)));
        });
    }

    [Test]
    public void ErrorPage_ShouldInitializeCorrectly()
    {
        // Arrange
        const string url = "https://example.org";
        const string error = "dummy error";

        // Act
        var page = Page.ErrorPage(url, error);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(page.Url, Is.EqualTo(url));
            Assert.That(page.Title, Is.Null);
            Assert.That(page.Links, Is.Empty);
            Assert.That(page.Error, Is.EqualTo(error));
            Assert.That(page.HasError, Is.True);
            Assert.That(page.Success, Is.False);
            Assert.That(page.Timestamp, Is.Not.EqualTo(default(DateTimeOffset)));
        });
    }
}