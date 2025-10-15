# 🕸️ Parallel Web Scraper: Comparing Parallel Programming in Go and C#
[![Build & Test C#](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/csharp.yml/badge.svg)](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/csharp.yml)
[![Build & Test Go](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/go.yml/badge.svg)](https://github.com/SimonLiebers-Dev/concepts-of-programming-languages/actions/workflows/go.yml)

> This project was built as part of the course *Concepts of Programming Languages* at TH Rosenheim.

## 📚 Table of Contents

1. [Overview](#overview)  
2. [Architecture](#architecture)  
3. [C# Implementation (.NET 9)](#csharp-implementation)  
   - [Setup](#setup-1)  
   - [Configuration](#configuration-1)  
   - [Running](#running-1)  
   - [Testing](#testing-1)  
4. [Go Implementation](#go-implementation)  
   - [Setup](#setup-1)  
   - [Configuration](#configuration-2)  
   - [Running](#running-2)
   - [Testing](#testing-2)  
7. [License](#license)

## 🧩 Overview

The **Parallel Web Scraper** demonstrates two implementations of a parallel web scraping application — one in **C# (.NET 9)** and one in **Go 1.25** — to explore and compare the concepts of **parallel programming**, **asynchronous execution**, and **configuration-driven architectures** across different languages.

The main focus of this project lies on the **C# implementation**, which serves as a fully featured, extensible, and modular architecture. The **Go version** replicates its functionality to serve as a direct comparison for evaluating concurrency programming in both languages.

### ✨ Features

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