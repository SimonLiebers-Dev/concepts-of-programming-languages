//
//  ScrapeOutputManager.swift
//  swift-scraper
//
//  Created by Simon Liebers on 03.10.25.
//

import Foundation

struct ScrapeOutputManager {
    private static let dateFormatter: DateFormatter = {
        let df = DateFormatter()
        df.dateFormat = "HH:mm:ss"
        return df
    }()

    /// Called when scraping a URL starts.
    static func scrapeStart(url: String) {
        let ts = timestamp()
        print("⏳ [\(ts)] Starting scrape: \(url)")
    }

    /// Called when scraping finishes successfully.
    static func scrapeSuccess(url: String) {
        let ts = timestamp()
        print("✅ [\(ts)] Finished successfully: \(url)")
    }

    /// Called when scraping finishes with error.
    static func scrapeError(url: String, error: String) {
        let ts = timestamp()
        print("❌ [\(ts)] Error scraping \(url): \(error)")
    }

    /// Helper to generate a formatted timestamp.
    private static func timestamp() -> String {
        return dateFormatter.string(from: Date())
    }
}
