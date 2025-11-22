using WebScraper.Core.Models;

namespace WebScraper.Core.Tests.Models;

[TestFixture]
public class PageTests
{
    [Test]
    public void SuccessPage_ShouldCreatePageWithExpectedValues()
    {
        // Arrange
        const string url = "https://example.com";
        const string title = "Example Page";
        var links = new[] { "https://a.com", "https://b.com" };
        var images = new[] { "img1.png", "img2.png" };
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        var page = Page.SuccessPage(url, title, links, images, timestamp);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(page.Url, Is.EqualTo(url));
            Assert.That(page.Title, Is.EqualTo(title));
            Assert.That(page.Links, Is.EquivalentTo(links));
            Assert.That(page.Images, Is.EquivalentTo(images));
            Assert.That(page.Timestamp, Is.EqualTo(timestamp).Within(TimeSpan.FromMilliseconds(1)));
            Assert.That(page.Success, Is.True);
            Assert.That(page.ErrorMessage, Is.Null);
        }
    }

    [Test]
    public void SuccessPage_ShouldUseEmptyCollections_WhenLinksOrImagesNull()
    {
        // Act
        var page = Page.SuccessPage("https://example.com", "Title");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(page.Links, Is.Empty);
            Assert.That(page.Images, Is.Empty);
        }
    }

    [Test]
    public void ErrorPage_ShouldCreatePageWithExpectedValues()
    {
        // Arrange
        const string url = "https://example.com";
        const string error = "Network failure";
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        var page = Page.ErrorPage(url, error, timestamp);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(page.Url, Is.EqualTo(url));
            Assert.That(page.Title, Is.Null);
            Assert.That(page.Links, Is.Empty);
            Assert.That(page.Images, Is.Empty);
            Assert.That(page.Timestamp, Is.EqualTo(timestamp).Within(TimeSpan.FromMilliseconds(1)));
            Assert.That(page.Success, Is.False);
            Assert.That(page.ErrorMessage, Is.EqualTo(error));
        }
    }

    [Test]
    public void ErrorPage_ShouldUseCurrentTimestamp_WhenNotProvided()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;

        // Act
        var page = Page.ErrorPage("https://example.com", "error");

        // Assert
        var after = DateTimeOffset.UtcNow;
        Assert.That(page.Timestamp, Is.InRange(before, after));
    }

    [Test]
    public void SuccessPage_ShouldUseCurrentTimestamp_WhenNotProvided()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;

        // Act
        var page = Page.SuccessPage("https://example.com", "title");

        // Assert
        var after = DateTimeOffset.UtcNow;
        Assert.That(page.Timestamp, Is.InRange(before, after));
    }
}