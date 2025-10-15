using WebScraper.Cli.Util;

namespace WebScraper.Cli.Tests.Util;

[TestFixture]
public class LayoutUtilsTests
{
    private StringWriter _output = null!;
    private TextWriter _originalOut = null!;
    private ConsoleColor _originalColor;

    [SetUp]
    public void SetUp()
    {
        _originalOut = Console.Out;
        _output = new StringWriter();
        Console.SetOut(_output);
        _originalColor = Console.ForegroundColor;
    }

    [Test]
    public void PrintHeader_ShouldOutputAsciiAndInfoLines()
    {
        // Act
        LayoutUtils.PrintHeader();
        var output = _output.ToString();

        // Assert
        Assert.That(output, Does.Contain("_____"), "Header ASCII art should contain underscores.");
        Assert.That(output, Does.Contain("Parallel Web Scraper"), "Should print scraper name line.");
        Assert.That(output, Does.Contain("Simon Liebers"), "Should print author name.");
        Assert.That(output, Does.Contain("GitHub"), "Should contain GitHub link.");
        Assert.That(output, Does.Contain(".NET 9").And.Contain("C#"), "Should contain platform info.");
    }

    [Test]
    public void PrintHeader_ShouldResetConsoleColor()
    {
        // Arrange
        Console.ForegroundColor = ConsoleColor.Red;

        // Act
        LayoutUtils.PrintHeader();

        // Assert
        Assert.That(Console.ForegroundColor, Is.EqualTo(_originalColor),
            "PrintHeader() should reset console color after writing header.");
    }

    [Test]
    public void PrintSeparator_ShouldPrintLineOfSpecifiedWidth()
    {
        // Arrange
        const int width = 50;

        // Act
        LayoutUtils.PrintSeparator(width);
        var output = _output.ToString().TrimEnd();

        // Assert
        Assert.That(output, Has.Length.EqualTo(width), "Separator line length should match width parameter.");
        Assert.That(output.All(c => c == '─'), Is.True, "Separator should only contain box-drawing characters.");
    }

    [Test]
    public void PrintSeparator_ShouldDefaultToWidth100()
    {
        // Act
        LayoutUtils.PrintSeparator();
        var output = _output.ToString().TrimEnd();

        // Assert
        Assert.That(output, Has.Length.EqualTo(100), "Default separator width should be 100 characters.");
    }

    [TearDown]
    public void TearDown()
    {
        Console.SetOut(_originalOut);
        Console.ForegroundColor = _originalColor;
        _output.Dispose();
    }
}