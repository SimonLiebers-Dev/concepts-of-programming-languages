//
//  Page.swift
//  swift-scraper
//
//  Created by Simon Liebers on 03.10.25.
//

import Foundation

struct Page: Codable {
    /// The original URL of the page
    let url: String

    /// The title of the page
    let title: String

    /// All links found on the page
    let links: [String]

    /// When the page was scraped
    let timestamp: Date

    /// Error message
    let error: String?

    // Custom initializer to create Page
    init(
        url: String,
        title: String = "",
        links: [String] = [],
        timestamp: Date = Date(),
        error: String? = nil
    ) {
        self.url = url
        self.title = title
        self.links = links
        self.timestamp = timestamp
        self.error = error
    }
}
