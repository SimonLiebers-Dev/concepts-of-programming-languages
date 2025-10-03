//
//  Runner.swift
//  swift-scraper
//
//  Created by Simon Liebers on 03.10.25.
//

import Foundation

struct Runner {
    static func scrapeSequential(urls: [String]) -> [Page] {
        var results: [Page] = []

        for url in urls {
            let group = DispatchGroup()
            group.enter()

            var result: Page?

            Task {
                ScrapeOutputManager.scrapeStart(url: url)
                result = await Scraper.scrape(url: url)
                group.leave()
            }

            group.wait()

            if let page = result {
                if page.error != nil {
                    ScrapeOutputManager.scrapeError(url: page.url, error: page.error!)
                } else {
                    ScrapeOutputManager.scrapeSuccess(url: page.url)
                }

                results.append(page)
            }
        }

        return results
    }

    static func scrapeParallel(urls: [String]) async -> [Page] {
        let collector = PageCollector()

        await withTaskGroup(of: Void.self) { group in
            for url in urls {
                group.addTask {
                    ScrapeOutputManager.scrapeStart(url: url)
                    let page = await Scraper.scrape(url: url)

                    if page.error != nil {
                        ScrapeOutputManager.scrapeError(url: url, error: page.error!)
                    } else {
                        ScrapeOutputManager.scrapeSuccess(url: url)
                    }
                    await collector.append(page)
                }
            }
        }

        return await collector.getAll()
    }
}

private actor PageCollector {
    private(set) var pages: [Page] = []

    func append(_ page: Page) {
        pages.append(page)
    }

    func getAll() -> [Page] {
        return pages
    }
}
