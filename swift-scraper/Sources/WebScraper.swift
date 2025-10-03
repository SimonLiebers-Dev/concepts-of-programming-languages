//
//  WebScraper.swift
//  swift-scraper
//
//  Created by Simon Liebers on 03.10.25.
//

import Foundation

let urlsFile = FileManager.default.currentDirectoryPath + "/urls.json"

// MARK: - Entry Point
@main
struct WebScraper {
    static func main() async {
        Layout.printHeader()
        Layout.printSeparator()

        let urls: [String]

        do {
            urls = try JsonUtils.getUrlsFromFile(configFile: urlsFile)
        } catch {
            print("❌ Error loading URLs: \(error)")
            return
        }

        if urls.isEmpty {
            print("⚠️ No URLs configured.")
            print(
                "📄 Please add URLs to 'urls.json' before running the scraper."
            )
            return
        }

        print("✅ Loaded \(urls.count) URLs from \(urlsFile)")
        Layout.printSeparator()

        // MARK: - Menu loop
        var choice: Int?
        let validChoices = [1, 2]

        while choice == nil {
            print("Choose scraping mode:")
            print("1 - Sequential")
            print("2 - Parallel")
            Layout.printSeparator()

            if let input = readLine(),
                let parsed = Int(input.trimmingCharacters(in: .whitespaces))
            {
                if validChoices.contains(parsed) {
                    choice = parsed
                } else {
                    Layout.printSeparator()
                    print("❌ Invalid choice. Please enter 1 or 2.")
                    Layout.printSeparator()
                }
            } else {
                Layout.printSeparator()
                print("❌ Invalid input. Please enter 1 or 2.")
                Layout.printSeparator()
            }
        }

        Layout.printSeparator()

        // MARK: - Scrape
        let start = Date()
        var pages: [Page] = []

        if choice == 1 {
            print("🚀 Running sequential scraper...")
            Layout.printSeparator()
            pages = Runner.scrapeSequential(urls: urls)
        } else {
            print("🚀 Running parallel scraper...")
            Layout.printSeparator()
            pages = await Runner.scrapeParallel(urls: urls)
        }

        // MARK: - Report
        let duration = Date().timeIntervalSince(start)
        let successCount = pages.filter { $0.error == nil }.count
        let errorCount = pages.count - successCount

        print(
            "✅ \(successCount) successful | ❌ \(errorCount) failed | ⏱️ Duration: \(String(format: "%.2fs", duration))"
        )
        Layout.printSeparator()

        // MARK: - Save
        while true {
            print(
                "💾 Do you want to save the results to a file? (y/n): ",
                terminator: ""
            )
            guard
                let answer = readLine()?.lowercased().trimmingCharacters(
                    in: .whitespacesAndNewlines
                )
            else {
                print("Please type 'y' or 'n'")
                continue
            }

            if answer == "y" {
                do {
                    let filename = try JsonUtils.saveResultsToFile(pages: pages)
                    print("✅ Results saved to: \(filename)")
                } catch {
                    print("❌ Error saving file: \(error)")
                }
                break
            } else if answer == "n" {
                print("ℹ️ Results not saved.")
                break
            } else {
                print("Please type 'y' or 'n'")
            }
        }
    }
}
