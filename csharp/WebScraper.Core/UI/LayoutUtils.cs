namespace WebScraper.Core.UI;

public static class LayoutUtils
{
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

    private static readonly string[] InfoLines =
    [
        "📦  Parallel Web Scraper (CLI Tool)",
        "👨‍💻  Developer: Simon Liebers",
        "🌐  GitHub: https://github.com/SimonLiebers-Dev/concepts-of-programming-languages",
        "🧠  Built with: C# (.NET 9)"
    ];

    /// <summary>
    /// Renders the ASCII header and metadata.
    /// </summary>
    public static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(AsciiHeader);
        Console.ResetColor();

        foreach (var line in InfoLines)
            Console.WriteLine(line);
    }

    /// <summary>
    /// Prints a horizontal separator line across the console width (default 100).
    /// </summary>
    public static void PrintSeparator(int width = 100)
    {
        var line = string.Concat(Enumerable.Repeat('─', width));
        Console.WriteLine(line);
    }
}