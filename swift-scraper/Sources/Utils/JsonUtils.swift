//
//  JsonUtils.swift
//  swift-scraper
//
//  Created by Simon Liebers on 03.10.25.
//

import Foundation

enum UrlReadError: Error {
    case fileCreationFailed
    case fileReadFailed
    case invalidJsonFormat
}

enum UrlWriteError: Error {
    case writeFailed
}

struct JsonUtils {
    static func getUrlsFromFile(configFile: String) throws -> [String] {
        let fileManager = FileManager.default
        let url = URL(fileURLWithPath: configFile)

        if !fileManager.fileExists(atPath: configFile) {
            let emptyJson = "[]".data(using: .utf8)!
            do {
                try emptyJson.write(to: url)
                return []
            } catch {
                throw UrlReadError.fileCreationFailed
            }
        }

        let data: Data
        do {
            data = try Data(contentsOf: url)
        } catch {
            throw UrlReadError.fileReadFailed
        }

        do {
            let urls = try JSONDecoder().decode([String].self, from: data)
            return urls
        } catch {
            throw UrlReadError.invalidJsonFormat
        }
    }

    static func saveResultsToFile(pages: [Page]) throws -> String {
        let timestamp = UInt64(Date().timeIntervalSince1970 * 1000)
        let filename = "scrape-results-\(timestamp).json"
        let url = URL(fileURLWithPath: filename)

        do {
            let data = try JSONEncoder().encode(pages)
            try data.write(to: url)
            return filename
        } catch {
            throw UrlWriteError.writeFailed
        }
    }
}
