import XCTest

@testable import swift_scraper

final class ScraperTests: XCTestCase {
    func testParseHtmlExtractsTitleAndLinks() throws {
        // Arrange
        let html = """
            <html>
                <head><title> Test Page </title></head>
                <body>
                    <a href="https://example.com">Example</a>
                    <a href="/local">Local Link</a>
                    <a>No href</a>
                    <a href="">Empty href</a>
                </body>
            </html>
            """

        // Act
        let (title, links) = try Scraper.parseHtml(html: html)

        // Assert title matches the html input
        XCTAssertEqual(title, "Test Page")

        // Assert links are correctly extracted from html
        XCTAssertEqual(links.count, 2)
        XCTAssertTrue(links.contains("https://example.com"))
        XCTAssertTrue(links.contains("/local"))
    }
}
