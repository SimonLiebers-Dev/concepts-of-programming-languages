using WebScraper.Core.Extensions;

namespace WebScraper.Core.Tests.Extensions;

[TestFixture]
public class StringExtensionsTests
{
    [TestCase("A", ExpectedResult = 1)]
    [TestCase("AB", ExpectedResult = 2)]
    [TestCase("📦", ExpectedResult = 2)]
    [TestCase("👨‍💻", ExpectedResult = 2)]
    [TestCase("Hello📦", ExpectedResult = 7)]
    [TestCase("", ExpectedResult = 0)]
    public int GetDisplayWidth_ShouldReturnExpectedWidth(string input)
    {
        return input.GetDisplayWidth();
    }

    [Test]
    public void PadDisplayRight_ShouldPadWithSpaces_WhenStringIsShorter()
    {
        var result = "A".PadDisplayRight(5);

        Assert.That(result, Is.EqualTo("A    "));
        Assert.That(result, Has.Length.EqualTo(5));
    }

    [Test]
    public void PadDisplayRight_ShouldNotPad_WhenStringIsLongEnough()
    {
        var result = "ABCDE".PadDisplayRight(5);

        Assert.That(result, Is.EqualTo("ABCDE"));
    }

    [Test]
    public void PadDisplayRight_ShouldHandleEmojiProperly()
    {
        var result = "📦".PadDisplayRight(4);

        Assert.That(result, Is.EqualTo("📦  "));
    }

    [Test]
    public void TruncateText_ShouldReturnOriginal_WhenShorterThanMaxLength()
    {
        const string input = "Hello";
        var result = input.TruncateText(10);

        Assert.That(result, Is.EqualTo("Hello"));
    }

    [Test]
    public void TruncateText_ShouldTruncate_WhenLongerThanMaxLength()
    {
        const string input = "HelloWorld";
        var result = input.TruncateText(8);

        Assert.That(result, Is.EqualTo("Hello..."));
    }

    [Test]
    public void TruncateText_ShouldUseCustomIndicator()
    {
        const string input = "HelloWorld";
        var result = input.TruncateText(8, "[cut]");

        Assert.That(result, Is.EqualTo("Hel[cut]"));
    }

    [Test]
    public void TruncateText_ShouldReturnExactLength_WhenEqualToMaxLength()
    {
        const string input = "Hello";
        var result = input.TruncateText(5);

        Assert.That(result, Is.EqualTo("Hello"));
    }

    [Test]
    public void TruncateText_ShouldHandleEmptyString()
    {
        var result = string.Empty.TruncateText(5);

        Assert.That(result, Is.EqualTo(string.Empty));
    }
}