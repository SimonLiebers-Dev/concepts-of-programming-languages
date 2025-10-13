using WebScraper.Core.Models;

namespace WebScraper.Core.Tests.Models;

[TestFixture]
public class ParserResultTests
{
    [Test]
    public void Constructor_ShouldSetAllProperties()
    {
        // Arrange
        var title = "Example Page";
        var links = new[] { "https://a.com", "https://b.com" };
        var images = new[] { "img1.png", "img2.jpg" };

        // Act
        var result = new ParserResult(title, links, images);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Title, Is.EqualTo(title));
            Assert.That(result.Links, Is.EqualTo(links));
            Assert.That(result.Images, Is.EqualTo(images));
        });
    }

    [Test]
    public void WithExpression_ShouldCreateNewInstance_WithUpdatedValue()
    {
        // Arrange
        var original = new ParserResult("Old Title", ["a"], ["img"]);

        // Act
        var updated = original with { Title = "New Title" };

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updated.Title, Is.EqualTo("New Title"));
            Assert.That(updated.Links, Is.EqualTo(original.Links));
            Assert.That(updated.Images, Is.EqualTo(original.Images));
            Assert.That(updated, Is.Not.SameAs(original));
        });
    }

    [Test]
    public void ShouldAllowEmptyArrays()
    {
        // Act
        var result = new ParserResult("Empty Test", [], []);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Links, Is.Empty);
            Assert.That(result.Images, Is.Empty);
        });
    }

    [Test]
    public void ShouldHandleNullValues_IfExplicitlyPassed()
    {
        // Act
        var result = new ParserResult("Null Test", null!, null!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Links, Is.Null);
            Assert.That(result.Images, Is.Null);
        });
    }
}