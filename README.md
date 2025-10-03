# WebScraper: Comparing Parallel Programming in Go and Swift
[![Build & Test Swift](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/swift.yml/badge.svg)](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/swift.yml)
[![Build & Test Go](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/go.yml/badge.svg)](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/go.yml)

> This project was built as part of the course *Concepts of Programming Languages* at TH Rosenheim.

## 🧠 Project Summary
This project demonstrates a parallel web scraping application implemented in two different programming languages:
- **Go 1.25** — using goroutines and channels
- **Swift 6.1** — using Swift Concurrency with `async/await` and task groups

The goal is to compare both languages in terms of:
- **Parallel programming capabilities and ease of use**
- **Performance**

Both versions share the same logic and structure to ensure a fair comparison.

## 🚀 Features
- Reads a list of URLs from a `urls.json` file
- Scrapes each page for:
  - Page title
  - All hyperlinks
- Progress reporting
- Option to run in **sequential or parallel** mode
- Graceful error handling (invalid URLs, timeouts, etc.)
- Saves results as JSON (optional)

## 📁 Project Structure (Both Go and Swift)
```
root/
├── go-scraper/         # Go implementation
├── swift-scraper/      # Swift implementation
├── LICENSE
└── README.md           
```

## ▶️ How to Run the Projects

### Go
#### Requirements
- Go 1.25+

#### Run
```bash
cd go-scraper
go run main.go
```

#### Run tests
```bash
cd go-scraper
go test ./...
```

### Swift
#### Requirements
- Swift 6.1+
- Xcode or Swift command line tools

#### Run
```bash
cd swift-scraper
swift run
```

#### Run tests
```bash
cd swift-scraper
swift test
```

## ⚙️ Comparison

| Feature                         | Go                                      | Swift                                  |
|--------------------------------|------------------------------------------|----------------------------------------|
| Concurrency model              | Goroutines + Channels                    | Structured Concurrency (`async/await`) |
| Task spawning                  | `go func(...)`                           | `Task { ... }` / `withTaskGroup`       |
| Synchronization                | Channels / WaitGroups                    | Actor isolation / `await`              |
| Error handling                 | Error return values                      | `try`/`await` + Result types           |
| Code verbosity                 | Low                                      | Medium (more structured)               |

### 🧠 Conclusion
- **Go** feels lighter and more natural for parallelism.
- **Swift** offers more safety and structure, but with more boilerplate.
- Both can be used to build robust CLI tools with good error handling and responsiveness.

## 📚 References
- [Go Documentation - Goroutines](https://go.dev/doc/effective_go#goroutines)
- [Swift Documentation - Concurrency](https://docs.swift.org/swift-book/documentation/the-swift-programming-language/concurrency/)