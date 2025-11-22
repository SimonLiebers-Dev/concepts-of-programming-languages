# 🕸️ Parallel Web Scraper: Comparing Parallel Programming in Go and C#

[![GitHub Release](https://img.shields.io/github/v/release/SimonLiebers-Dev/concepts-of-programming-languages)](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/releases)
[![GitHub License](https://img.shields.io/github/license/SimonLiebers-Dev/concepts-of-programming-languages)](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/blob/main/LICENSE)

![Static Badge](https://img.shields.io/badge/-C%23%2014-blueviolet?logo=data%3Aimage%2Fpng%3Bbase64%2CiVBORw0KGgoAAAANSUhEUgAAAOQAAAEACAYAAACj9dqtAAAAAXNSR0IArs4c6QAADmlJREFUeF7tndt666gSBsfv%2F9BrPidx4oMkoOmGPtS%2BNSCo%2Fsstydlrbv%2FxP%2FcE%2Fv37909jk7fb7aaxDmvYEaBAdmynV9YS8X0jiDldGrMFENIM7dzCVjI%2B7wox52pkMRshLahOrrlCxvsWEXKyUAbTEdIAqnTJVSJyCyutkP08hLRnfHmFXRKebYquuTcQCLmRvzcZHyiQcl8oEHIDe68iciu7IQxvl0TIhTWIIiJiLgwFQu6DjZD72Ee5Mh3SsFJRBWwh4RmzRUj%2BOULK2YV6e6p9TKTUJvq9HkIacM3aGXm2NAgLz5B2UKuIiJh2GaJDKrCtKiJiKoSHDqkHERGPWfJ8Kc8YHVLADhH7oCFmH6fnUQg5yAwZx4Ah5SCvseG1RyOjvP6I2ceODtnBCRE7IHUMQco2JIQ8YYSE7fDMjEDOkxdiM1CzzkXGNZVFyk%2FOdMgnJoi4RkR%2BvzznjJA%2FbJBxj4wvr%2Fz5Zyr5W1ZE3C8iUv4RKNkhkdCXhGe7qfiMWU5IZIwh42OX1aQsJSQyxpKx4q1sCSERMa6I1aRMLSQi5hCx0s8kKYVExJwiHp0q2zNmKiERsY6IWW9l0wiJjDVlzPY2NryQiFhbxGzPl6GFREZkzPZcGU5IJETCEQLRXvqEERIRR2LI2Ki3siGEREYE0yAQoVu6FxIZNaLIGlHexroVEhGRyJKA127pSkgktIwga58R8CSnGyGREWF2EvAi5XYhEXFnDLm2t7exW4VERoTwSmBXx9wmJDJ6jSL7uhNILyQCEvTIBFYJuqRDImPkKLL3lb9hmguJjAQ6EwHrTmkmJCJmiiFnWfU2Vl1IRCS8lQhod0w1IRGxUgw5q1XHnBYSEQknBP4IzHbMKSGRkShC4JPAjJRiIZFxTxQlxaZW62slqdPXHySMbpXijhKTjZcWdORq1HKElmzsaB27haR4soL0zBotWs%2Ba0jHUWUquPa%2Bnzk0hKVAb9MiInqKMrLdiLBnQo9yq%2F6WQFEKnEK0i6FzFfhXyoMP4Kg%2BnQgJ%2FHn4WEY9IkI%2B5fJxl41BIYNvAnlvV32xyMleTIyk%2FhASyHHLmjtiiQm5ahI4%2Ff8%2FMi5BAHYdaWcIzWuRoLEfPGULIMXa%2FoxHxGhxS9gfrUEgA9gFExD5Oj1Hkqo%2FXI1e%2FHRJwbXDI2GbEG1kZoxchkbENERnbjFojyNk1oXvGbkA6h4SELcVkn5O5i8wBp%2B91tCx6zOJN7FgG6JAHvOiMYyGSjqYZfJJDyDcmyCjVSzYPKV%2B5IeQPD0SUCaU1CzG%2FSSLkxn82XivMWdZBSoTc9t9wyCKR9jmqS1m6Q3Kbqq2TznqVpSwrJDLqyGO1SlUpSwqJjFYa6a5bUcpyQiKjrjTWq1WTspSQyGitj836laQsIyQy2siyatUqUpYQEhlXaWN7nQpSphcSGW0lWb16dilTC4mMq3Wxvx5C2jM2uQIymmB1sWhmKdN2SIR04Y7ZJrJKmVJIZDTzwNXCGaVMJyQyunLGdDMIaYp3fnFknGcYcYVMYqbqkAgZUaf5PSPkPEOTFRDSBKv7RRHSYYmQ0WFRFm4pi5QpblmzymgZskzMLDkt%2FE75ulR4ITMF616Q1eGKzG81qxVyIuQKyh3X2B2uSGLuZtVRTvGQ0EJGCtFZhbyFyztTb7zE5p1MREhtogPreQ2XVym98hooeXNoWCG9hqZFPFqodnOOxqtV%2F9bnCNkipPh51HDtkjIqr5nIIOQMvYG50cO1UsrorAZi8TE0pJArwzED9zE3S8CsuWfhNJMZhJyh1zE3W8ispMzGqSMah0PCCWkVCCnAKD9naJ9Pow5I%2BFkVhNRO6s962cM2I2R2NjORCiXkTAhmII3OrRI4aT2q8BnNzX08QkqoXcypFrYRKauxkUQLISXUTuZUDVxLyqpcJNEKI2Sr6JLDa86pHrqj%2BlRnIskXQkqovc0heN9AHlLCQx6qEELSHeUFjjZT2mnf50X9UkBIhcRGLb7C0dWXkIolnad%2BgMkFEXISIDJOAnybLhVLOk939%2FOrIeQEQ2ScgHcyVSqWdJ7%2BCeZWRMgJfgg5Ae9gqvT58fmF0mPZqLVByIlMRS36xJFNp0qFlM4zPYxwcYQUgkNGIbiLaVKxpPP0TzC%2Fonshvf7kgZDz4XtfQSqWdJ7%2BCeZXREgBQ2QUQOuYIhVLOq9jS8uHIKQAOUIKoHVMkYolndexpeVDEFKAHCEF0N6m7HgUiVA3hBzMVoSiDh5py3CEPMaOkINxRMhBYCfDETKgkDuK1oobQrYI9X2%2Bo7YRaue6Q%2B4oWitOEYraOoPHz6UvZqTzPDK47wkhByuDkIPABoZL%2Fh4VIQcAzw6lQ84SjDUfIemQw4mlQw4j656AkAjZHZbHQIQcRtY9QUPI6PXhGbI7Lt8Doxd88LhLhyMkHXI4cAg5jKxrgvTljETirg1tGkSHHASPkIPAOodLhJTM6dzOtmEIOYgeIQeBdQ6XyCWZ07mdbcMQchA9Qg4C6xwukUsyp3M724Yh5CB6hBwE9jZ8x2%2FLkWqGkIP5ilTcwaMtGY6Q15gRcjCGCDkIjA45BMy1kPeT7PhGvSKIkEP5%2Bhi8o56RaoaQg%2FmKVNzBoy0ZjpCBb1npkEsc2X4RydtSyZztB%2B3YAB2yA9LzEDrkILCO4RK5JHM6trJ9CEIOlgAhB4F1DJfIJZnTsZXtQxBSUAKkFEC7mCKRSzJHd9c2qyGkgCtCCqANCNnDFyF1azC02o43c60N9oSmtQaffxOQ%2FD82JHMi8HbfIY8K5gEsQupVQSKXZI7eju1WQkghW4QUgnubJr31REgd%2FqJVPN6y3g%2BClKJyvkySCCmZM7%2FTNSvQISc4I%2BQEvJ%2BpErkkc%2BZ3umYFhJzkjJRzACVySebM7XLdbIRUYI2UChBZ4vuNc5QweX2O5FkSkzQJIKQCzShfagpHvVzi8aUJDznpMEJ%2BtfPb7SY%2Fqu3M6iHM%2FFxnm5zX1RFSkXZVKVtflFW5SKKFkBJqF3Oqha8l4zOqamwk0QolpPfb1kcBqgRvREbE7NMTIfs4DY%2FKLqVURt5KX0cpnJB0yeHvBpMJM0JWu5MYKQBCjtASjM3WKTVEPMKYjZMgKl9TQgoZpUtm6wRWMmbjJJURIWfIDc6N3gGsZeSlzzcBOuSgWDPDo0q5UsbqYiLkjGGCudGk3CVj1dvYsEJGe46M9CJjt4Rn33PRvswE39dxb1kzCHk%2Fg7eQeZWxSscM3SGzSOlFTO8yVpASISX3FcZzVnbNKBJGuuWfiUd4ITN1yfdCWooZWcSVnGbkksxNIWRmKXuK%2BixuJtF6zp7tNjaNkNWlHAlvtrGWdxKrWSHkauJcT50AQqoj1Vmw6u2aDr24qyCk49ohpePiGGwtk4xfj13ZDsSzpEHqnS6ZMrsZD4WUTg1S3Fba3GY9GFIqpt%2FZUqkzm%2Fpwjv8dV2cZD7Wd1JnNfDi6ZCjPujabPq%2FZD4iUXTkPMahEViscEilD%2BHa5yTI5rXJQpIwtZZWcpvwd8ip6%2FOFAPDGryJj2DwNakUPKFiEfn1cS8UG8XIf8PTg%2Fifiw7mIXCOm%2BRLobpFPq8tRcraKMZW9Zn4ODlJoa6axVVUaE%2FMkPUuqIpLFKZRkR8ilBSKmhk3yN6iKWf6lzFB2klAs1MxMZ%2F%2BiVfcvKb5UzCunNRcZXlgh5ki26pZ50Zysh4ycZhLzIHVLaSImI51xv948AdB08xNQTk6xdyPgcNEC1Q4eYbUbcnsoZ3fP11SHpkv0QkbKfFbnqZ%2FXI1a%2BQwOuHdx%2BJmG1e3HW1GT1GIGQ%2Fq8uRiHmMBxn7A%2FacoZcOSZfsh%2Fg%2BEjF5OShJz3tuPoRESgnW1zmV5KQTyvNylJNDIZFSDvl5ZmYxEXEuI2fZOBUSKeeAZxUTEedzcfVFfSkkUs7Dz%2FKsiYg6WWjdNTWFfGyDgugU5GiVVpHsrswb0pVse%2BrcLSRiritdT%2BG0dsMXrRbJ43VGazksJGLaFvBq9dHiPq%2BFeGvrJq2VWEieL9cWmKvFISCV8X7CKSGRMk5I2OkaAjMyqgjJLeyaQnMV3wRmRXycbrpDvmPiWcV3cNidLgEtEc2EpGPqFpzVfBLQFtFcSMT0GSR2NUfASsRlQvLiZy4AzPZDwFpG1Zc6LWw8W7YI8blnAitkXCokL388x429vRNYJeDHdXeWgq65kz7XviJQUkieL5HCG4FdIi59qdMDnW7ZQ4kxVgR2i%2BhOSLqlVdRYt0XAi4xbX%2Bq0INExW4T4fIaAJwmfz6H%2Bp3MzkI7mIqY20drreRXR5S3rWVSQsrZEWqf3LqPrW9b3IiClVixrrhNBxlBCPmKEmDWFkp46ioihblm5lZXGsea8aBKGeqnTihQds0Wo1ueRZQx5y0q3rCVY72mji5jilpUXP71xzT0ui4ypOuRz5LiNzS3gyzPX0390OMOp3f9hwAxkxJyh53dupo74Tjm1kPxU4lcqyc4yi5jyGbJVZDpmi5DfzyvImPYZ8ipWSOlXuqOdVRGxZIfkFhYZvRMo8QxJx%2FQew7%2F9VeuIJV%2Fq9MSRW9keSrZjqstY8hmSbmkrlWR1RHy6Q5AAzD6Hbrmmwoj4ybn8M%2BRZ9JDSVkpkPOaLkB25Q84OSB1DkLANCSHbjH5HIOYArLehyNjHDiH7OCHlIKfHcEQcA4eQY7y%2BRtMp%2B6AhYx%2Bn51EIOc6MbtlghojyUCGknB1i8pyokJ7XJRBSEWnVW1k6ol6IEFKPZbmOiYj64UFIfaYlXvwgo01wENKGa2opkdEuNAhpx%2FZj5ajPmAi4LiQIuY512N8vEXJdSBByHetwL30QcX04EHI9c%2FdiIuK%2BUCDkPvYuX%2Fwg495AIORe%2Fi9X3%2FXSBwn9hAAh%2FdRi%2Ba0sIvorPkL6q8myW1mE9Fd8hPRXE%2FNOiYh%2Bi46QfmujLiYi%2Bi%2F2%2Fyv%2B%2F3ok7DtXAAAAAElFTkSuQmCC&link=https%3A%2F%2Fdotnet.microsoft.com%2Fen-us%2Fdownload%2Fdotnet%2F10.0)
![Static Badge](https://img.shields.io/badge/-.NET%2010-blueviolet?logo=dotnet&link=https%3A%2F%2Fdotnet.microsoft.com%2Fen-us%2Fdownload%2Fdotnet%2F10.0)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/SimonLiebers-Dev/concepts-of-programming-languages/csharp.yml?branch=main&label=C%23%20Build%20-%20Analyze%20-%20Test&cacheSeconds=20)](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/csharp.yml)

![Static Badge](https://img.shields.io/badge/-Go%201.25-blue?style=flat&logo=go&logoColor=ffffff)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/SimonLiebers-Dev/concepts-of-programming-languages/csharp.yml?branch=main&label=Go%20Build%20-%20Analyze%20-%20Test&cacheSeconds=20)](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/go.yml)


## Table of Contents

1. [Overview](#overview)  
2. [C# Implementation (.NET 10)](#csharp-implementation)  
   - [Architecture](#architecture-1)    
   - [Setup](#setup-1)  
   - [Configuration](#configuration-1)  
   - [Testing](#testing-1)  
3. [Go Implementation](#go-implementation)    
   - [Setup](#setup-2)   
   - [Configuration](#configuration-2)  
   - [Testing](#testing-2)  
4. [License](#license)

## Overview <a name="overview"></a>

The **Parallel Web Scraper** demonstrates two implementations of a parallel web scraping application — one in **C# (.NET 10)** and one in **Go 1.25** — to explore and compare the concepts of **parallel programming**, **asynchronous execution**, and **configuration-driven architectures** across different languages.

The main focus of this project lies on the **C# implementation**, which serves as a fully featured, extensible, and modular architecture. The **Go version** replicates its functionality to serve as a direct comparison for evaluating concurrency programming in both languages.

> This project was built as part of the course *Concepts of Programming Languages* at TH Rosenheim.

### Features

- **Parallel Scraping**  
  Executes multiple HTTP requests concurrently. The C# version offers two different strategies for parallel execution. Both strategies implement the same interface, allowing them to be easily interchanged through a generic setup method. The Go implementation leverages goroutines and channels for synchronization, following the [worker pool](https://gobyexample.com/worker-pools) pattern.

- **Modular Architecture**  
  Clear separation between core scraping logic (`WebScraper.Core`) and CLI application (`WebScraper.Cli`) in the C# implementation. The Go version mirrors this structure in a simpler form.

- **Configuration-driven Design**  
  Uses JSON-based configuration files (`appsettings.json` in C# and `config.json` in Go) to define the scraper’s behavior. The scraper’s behavior can be fully customized without modifying the source code. Configurable parameters include the path to the URL list file, the output directory for results, the level of concurrency, the HTTP request timeout, and the User-Agent string. Both the C# and Go versions load these settings at startup, ensuring flexible, consistent, and easily adjustable configurations.

- **Structured JSON Output**  
  Both implementations write results (list of urls with title, images, links) to a structured json file.

- **Error Handling**  
  Graceful error recovery with informative console output.

- **CLI Interface**  
  The C# CLI project implements a simple console UI that displays progress, timing, and summary results.

- **Testing Support**  
  Unit tests validate core logic, ensuring expected behavior.

## C# Implementation <a name="csharp-implementation"></a>

### Architecture <a name="architecture-1"></a>

The C# implementation of the **Parallel Web Scraper** is divided into two independent projects:

- **WebScraper.Core** — The reusable scraping library containing interfaces, services, and data models.  
- **WebScraper.Cli** — The command-line frontend that connects configuration, dependency injection, and user interaction.

This separation of concerns allows the scraping logic to be reused.

#### WebScraper.Core

[`WebScraper.Core`](csharp/WebScraper.Core/WebScraper.Core.csproj) defines all reusable and testable components for scraping websites. It provides abstractions for fetching, parsing, running concurrent tasks, and reporting progress.

![Core Class Diagram](.pics/classes_core.png)

> **Note:** The diagram intentionally omits explicit connections to the Page model and its related data flow.
While these links exist in the actual implementation, they would clutter the visual representation and make the class relationships less clear. The focus of this diagram is to highlight the core architectural structure. It shows how the main components interact (scraping, fetching, parsing, and progress reporting) without overloading it with data dependencies.

| Category | Key Types | Description |
|-----------|------------|-------------|
| **Scraping Logic** | [`IScraper`](csharp/WebScraper.Core/Scraping/IScraper.cs), [`DefaultScraper`](csharp/WebScraper.Core/Scraping/DefaultScraper.cs) | Defines the scraping workflow. The default implementation combines a `IHtmlFetcher` and `IHtmlParser` to extract metadata from HTML pages. |
| **Fetching** | [`IHtmlFetcher`](csharp/WebScraper.Core/Fetcher/IHtmlFetcher.cs), [`HtmlFetcher`](csharp/WebScraper.Core/Fetcher/HtmlFetcher.cs) | Handles HTTP requests with configurable timeout and user agent. |
| **Parsing** | [`IHtmlParser`](csharp/WebScraper.Core/Parser/IHtmlParser.cs), [`HtmlParser`](csharp/WebScraper.Core/Parser/HtmlParser.cs) | Extracts the title, links, and images from the downloaded HTML. |
| **Scrape Runners** | [`IScrapeRunner`](csharp/WebScraper.Core/Scraping/IScrapeRunner.cs), [`ParallelScrapeRunner`](csharp/WebScraper.Core/Scraping/ParallelScrapeRunner.cs), [`SemaphoreScrapeRunner`](csharp/WebScraper.Core/Scraping/SemaphoreScrapeRunner.cs) | Execute scraping sequentially or in parallel, providing interchangeable parallelization strategies. |
| **Progress Management** | [`IProgressBarManager`](csharp/WebScraper.Core/UI/IProgressBarManager.cs), [`ProgressBarManager`](csharp/WebScraper.Core/UI/ProgressBarManager.cs) | Provides console-based progress visualization and manages concurrent progress bars for each URL. |
| **Models** | [`Page`](csharp/WebScraper.Core/Models/Page.cs), [`ParserResult`](csharp/WebScraper.Core/Models/ParserResult.cs) | Immutable records representing scraping results and parsed data. |
| **Dependency Injection** | [`ServiceCollectionExtensions`](csharp/WebScraper.Core/DependencyInjection/ServiceCollectionExtensions.cs) | Registers all core services using a single generic extension method `AddWebScraperCore<TRunner>()`. |

#### WebScraper.Cli

`WebScraper.Cli` serves as the interactive entry point for the user. It provides configuration loading, mode selection, progress display, and result output.

![Cli Class Diagram](.pics/classes_cli.png)

| Category | Key Types | Description |
|-----------|------------|-------------|
| **Program & Bootstrap** | [`Program`](csharp/WebScraper.Cli/Program.cs), [`ServiceProviderUtils`](csharp/WebScraper.Cli/Util/ServiceProviderUtils.cs) | Entry point that initializes the DI container and launches the application. |
| **Application Logic** | [`Application`](csharp/WebScraper.Cli/App/Application.cs), [`IApplication`](csharp/WebScraper.Cli/App/IApplication.cs) | Orchestrates scraping sessions, handles user prompts, prints summaries, and manages flow control. |
| **Configuration** | [`ScrapeConfig`](csharp/WebScraper.Cli/Configuration/ScrapeConfig.cs), [`ConfigurationExtensions`](csharp/WebScraper.Cli/Extensions/ConfigurationExtensions.cs) | Loads and validates runtime configuration (concurrency, timeouts, directories, etc.). |
| **Utilities** | [`FileUtils`](csharp/WebScraper.Cli/Util/FileUtils.cs), [`LayoutUtils`](csharp/WebScraper.Cli/Util/LayoutUtils.cs) | Helper classes for file I/O and console formatting. |

The result is a clean, maintainable, and extensible architecture demonstrating modern .NET design patterns for parallel programming.

### Setup <a name="setup-1"></a>

Setting up the C# implementation requires the .NET 10 SDK. The project uses the built-in .NET CLI tools and has no external dependencies beyond those available via NuGet.

#### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- A terminal or IDE such as Visual Studio Code or Rider

#### Dependency Injection

All services are registered in [`ServiceProviderUtils.cs`](csharp/WebScraper.Cli/Util/ServiceProviderUtils.cs) using the extension method provided by the core library:

```csharp
services.AddWebScraperCore<ParallelScrapeRunner>();
```

This setup allows easy swapping of the runner by changing the generic parameter of [`AddWebScraperCore<>`](csharp/WebScraper.Core/DependencyInjection/ServiceCollectionExtensions.cs). The library offers two different implementations of the runner ([`ParallelScrapeRunner`](csharp/WebScraper.Core/Scraping/ParallelScrapeRunner.cs) and [`SemaphoreScrapeRunner`](csharp/WebScraper.Core/Scraping/SemaphoreScrapeRunner.cs)). You can use one of these implementations or provide a own implementation.

#### Build & Run

Clone the repository:

```bash
git clone https://github.com/SimonLiebers-Dev/concepts-of-programming-languages.git
cd concepts-of-programming-languages/csharp/WebScraper.Cli
```

By default the path to the URLs JSON-File is configured to `urls.json` which is located in `csharp/WebScraper.Cli`. Without further configuration, ensure that you are running the following steps inside the folder `csharp/WebScraper.Cli`. Otherwise the file will not be found (in this case a new empty JSON file is created for you).

Build and run the application:

```bash
dotnet build
dotnet run
```

The application will:
1. Display an ASCII banner and configuration summary.
2. Load all URLs from the provided JSON file.
3. Prompt for the scraping mode (sequential or parallel).
4. Execute the selected mode with live progress bars for each URL.
5. Summarize results and optionally save them to an output file.

The results are stored in the configured `ResultsDirectory` with a timestamped filename (e.g., `scrape-results-1760563678815.json`).

#### Example Output

![C# Cli](.pics/csharp_output.png)

### Configuration <a name="configuration-1"></a>

The C# scraper uses two configuration files — `appsettings.json` and `urls.json` — to define runtime behavior and scraping targets. This approach makes the scraper highly adaptable.

#### Configuration file: [appsettings.json](csharp/WebScraper.Cli/appsettings.json)

This file contains global scraper settings such as concurrency level, timeouts, output directory, and user agent string.

```jsonc
{
  "Scraper": {
    "UrlsFile": "urls.json",            // Path to the JSON file containing URLs to scrape
    "ResultsDirectory": "output",       // Directory where the results JSON file will be saved
    "Concurrency": 5,                   // Maximum number of concurrent scraping tasks
    "HttpTimeoutSeconds": 10,           // Timeout (in seconds) for HTTP requests
    "UserAgent": "ParallelScraper/1.0"  // Custom User-Agent string used for requests
  }
}
```

Configuration is automatically loaded and validated via [`ScrapeConfig`](csharp/WebScraper.Cli/Configuration/ScrapeConfig.cs) and [`ConfigurationExtensions`](csharp/WebScraper.Cli/Extensions/ConfigurationExtensions.cs):

```csharp
if (!ConfigurationExtensions.TryGetScrapeConfig(configuration, out var config, out var error))
{
    Console.WriteLine($"Configuration error: {error}");
    return;
}
```

#### Url file - Default: [urls.json](csharp/WebScraper.Cli/urls.json)

Defines the list of target URLs to scrape. Each URL is processed individually by the selected runner.

```jsonc
[
  "https://go.dev",
  "https://dotnet.microsoft.com"
]
```

The scraper reads this file through [`FileUtils.GetUrlsFromFileAsync()`](csharp/WebScraper.Cli/Util/FileUtils.cs).

#### Validation

Before execution, the configuration is validated by calling `ScrapeConfig.Validate()`. This ensures that all paths exist, concurrency levels are positive, etc. If validation fails, the CLI provides an error message and aborts execution safely.

### Testing <a name="testing-1"></a>

Testing in the C# implementation is organized into two independent projects to ensure a clear separation of concerns between the **core logic** and the **CLI layer**.

| Test Project | Purpose |
|---------------|----------|
| [**WebScraper.Core.Tests**](csharp/WebScraper.Core.Tests/WebScraper.Core.Tests.csproj) | Focuses on verifying the functionality of the reusable library components such as scraping, fetching, parsing, progress tracking, and concurrency handling. This ensures the core engine behaves correctly under different configurations and concurrency models. |
| [**WebScraper.Cli.Tests**](csharp/WebScraper.Cli.Tests/WebScraper.Cli.Tests.csproj) | Tests the console application layer, including configuration loading, integration with the core library, and basic flow control. It validates that the CLI correctly orchestrates services, processes user input, and handles output. |

Both test projects are designed to run independently, allowing developers to validate either the core library or the CLI in isolation. Together, they provide full coverage of the scraper’s runtime behavior.

All tests can be executed from the `csharp` directory with a single command:

```bash
dotnet test
```

This command automatically discovers and runs tests from both projects. Tests are automatically executed in the CI workflow via [`csharp.yml`](.github/workflows/csharp.yml).

## Go Implementation <a name="go-implementation"></a>

The **Go implementation** serves as a lightweight equivalent of the C# scraper and focuses primarily on showcasing **Go’s concurrency model** using goroutines and channels. While architecturally simpler, it preserves the same external behavior and configuration-driven execution model as the C# version.

### Setup <a name="setup-2"></a>

#### Prerequisites
- [Go 1.25+](https://go.dev/dl/)
- Terminal or IDE with Go module support (VS Code, GoLand, etc.)

#### Installation

Clone the repository and navigate to the Go project directory:

```bash
git clone https://github.com/SimonLiebers-Dev/concepts-of-programming-languages.git
cd concepts-of-programming-languages/go

# Download dependencies
go mod tidy
```

Build and run the scraper:

```bash
go run .
```

By default, the scraper expects configuration files (`config.json` and `urls.json`) to be located in the same directory as `main.go`.

#### Example Output

![C# Cli](.pics/go_output.png)

### Configuration <a name="configuration-2"></a>

The Go scraper reads its configuration from two JSON files: `config.json` and `urls.json`. Both mirror the structure and intent of the C# equivalents.

#### Configuration file: [config.json](go/config.json)

```jsonc
{
  "urlsFile": "urls.json",            // Path to the JSON file containing URLs to scrape
  "resultsDirectory": "output",       // Directory where the results JSON file will be saved
  "concurrency": 5,                   // Maximum number of concurrent scraping tasks
  "httpTimeoutSeconds": 10,           // Timeout (in seconds) for HTTP requests
  "userAgent": "ParallelScraper/1.0"  // Custom User-Agent string used for requests
}
```

#### Url file - Default: [urls.json](go/urls.json)

```jsonc
[
  "https://go.dev",
  "https://dotnet.microsoft.com"
]
```

### Testing <a name="testing-2"></a>

The Go implementation includes lightweight tests that verify correctness and scraping logic.

Run all tests with:

```bash
go test ./...
```

This command executes all tests within the `go` module, providing a summary of passed and failed tests.
Like the C# implementation, tests are also automatically executed in the CI workflow via [`go.yml`](.github/workflows/go.yml).

## References

- [Goroutines and Channels (Go Blog)](https://go.dev/doc/effective_go#goroutines)
- [Worker Pool Pattern in Go](https://gobyexample.com/worker-pools)
- [Asynchronous Programming with async and await (Microsoft Docs)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [PlantUML Documentation](https://plantuml.com/class-diagram)

## License <a name="license"></a>

The MIT License (MIT) - see [LICENSE](LICENSE) for more details