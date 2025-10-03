//
//  Scraper.swift
//  swift-scraper
//
//  Created by Simon Liebers on 03.10.25.
//

import Foundation
import SwiftSoup

struct Scraper {
    private static let userAgent = "Mozilla/5.0 (iPad; U; CPU OS 4_3_2 like Mac OS X; en-us) AppleWebKit/533.17.9 (KHTML, like Gecko) Mobile/8H7"

    static func scrape(url: String) async -> Page {
        let timestamp = Date()

        guard let u = URL(string: url) else {
            return Page(
                url: url,
                title: "",
                links: [],
                timestamp: timestamp,
                error: "Invalid URL"
            )
        }

        do {
            let (data, response) = try await fetchURL(u)

            guard let httpResponse = response as? HTTPURLResponse else {
                return Page(
                    url: url,
                    title: "",
                    links: [],
                    timestamp: timestamp,
                    error: "Invalid response"
                )
            }

            guard httpResponse.statusCode == 200 else {
                return Page(
                    url: url,
                    title: "",
                    links: [],
                    timestamp: timestamp,
                    error: "Status code: \(httpResponse.statusCode)"
                )
            }

            guard let html = String(data: data, encoding: .utf8) else {
                return Page(
                    url: url,
                    title: "",
                    links: [],
                    timestamp: timestamp,
                    error: "Failed to decode HTML"
                )
            }

            let (title, links) = try parseHtml(html: html)

            return Page(
                url: url,
                title: title,
                links: links,
                timestamp: Date(),
                error: nil
            )
        } catch {
            return Page(
                url: url,
                title: "",
                links: [],
                timestamp: timestamp,
                error: "Failed to fetch: \(error.localizedDescription)"
            )
        }
    }

    static func parseHtml(html: String) throws -> (String, [String]) {
        let doc = try SwiftSoup.parse(html)
        let title = try doc.title()

        var links: [String] = []
        let linkElements = try doc.select("a[href]")
        for link in linkElements.array() {
            let href = try link.attr("href")
            if !href.isEmpty {
                links.append(href)
            }
        }

        return (title.trimmingCharacters(in: .whitespacesAndNewlines), links)
    }

    private static func fetchURL(_ url: URL) async throws -> (Data, URLResponse) {
        var request = URLRequest(url: url)
        request.httpMethod = "GET"

        request.setValue(
            userAgent,
            forHTTPHeaderField: "User-Agent"
        )

        let (data, response) = try await URLSession.shared.data(for: request)
        return (data, response)
    }
}
