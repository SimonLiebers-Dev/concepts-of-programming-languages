using WebScraper.Core.Extensions;

namespace WebScraper.Cli.Util;

/// <summary>
/// Provides utility methods for rendering consistent visual elements in the CLI output,
/// such as headers and separator lines.
/// </summary>
/// <remarks>
/// The <see cref="LayoutUtils"/> class is responsible for maintaining a consistent
/// visual style across the console interface of the web scraper.  
/// It displays an ASCII art header, tool metadata, and formatted separators to improve readability.
/// </remarks>
public static class LayoutUtils
{
    /// <summary>
    /// The ASCII banner displayed at application startup.
    /// </summary>
    private const string AsciiHeader = """
                                       __          __  _     _____                                
                                       \ \        / / | |   / ____|                               
                                        \ \  /\  / /__| |__| (___   ___ _ __ __ _ _ __   ___ _ __ 
                                         \ \/  \/ / _ \ '_ \\___ \ / __| '__/ _' | '_ \ / _ \ '__|	
                                          \  /\  /  __/ |_) |___) | (__| | | (_| | |_) |  __/ |    
                                           \/  \/ \___|_.__/_____/ \___|_|  \__,_| .__/ \___|_|    
                                                                                 | |               
                                                                                 |_|               
                                       """;

    /// <summary>
    /// Informational lines displayed below the ASCII header.
    /// </summary>
    private static readonly string[] InfoLines =
    [
        CreateIconAndTextString("Parallel Web Scraper (CLI Tool)", "📦"),
        CreateIconAndTextString("Developer: Simon Liebers", "👨‍💻"),
        CreateIconAndTextString("GitHub: https://github.com/SimonLiebers-Dev/concepts-of-programming-languages", "🌐"),
        CreateIconAndTextString("Built with: C# (.NET 9)", "🧠")
    ];

    /// <summary>
    /// Prints the ASCII header and basic tool information to the console.
    /// </summary>
    /// <remarks>
    /// This method sets the console text color to cyan while rendering the ASCII header,
    /// then resets it before printing additional metadata such as the project name and author.
    /// </remarks>
    public static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(AsciiHeader);
        Console.ResetColor();

        foreach (var line in InfoLines)
            Console.WriteLine(line);
    }

    /// <summary>
    /// Prints a horizontal separator line across the console.
    /// </summary>
    /// <param name="width">
    /// The width of the separator line, measured in characters.  
    /// Defaults to <c>100</c>.
    /// </param>
    /// <remarks>
    /// Useful for visually separating sections of console output.
    /// </remarks>
    public static void PrintSeparator(int width = 100)
    {
        var line = string.Concat(Enumerable.Repeat('─', width));
        Console.WriteLine(line);
    }

    /// <summary>
    /// Creates pattern with icon and text to ensure the distance between
    /// icon and text is always the same
    /// </summary>
    /// <param name="text">Text to be printed</param>
    /// <param name="icon">Icon to be used before the text</param>
    /// <returns>Formatted string including icon and text</returns>
    public static string CreateIconAndTextString(string text, string icon)
    {
        return $"{icon.PadDisplayRight(4)}{text}";
    }
}