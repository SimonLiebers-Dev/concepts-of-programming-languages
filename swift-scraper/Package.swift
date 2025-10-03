// swift-tools-version: 6.1
// The swift-tools-version declares the minimum version of Swift required to build this package.

import PackageDescription

let package = Package(
    name: "swift-scraper",
    platforms: [
        .macOS(.v12)
    ],
    dependencies: [
        .package(url: "https://github.com/scinfu/SwiftSoup.git", from: "2.6.0")
    ],
    targets: [
        .executableTarget(
            name: "swift-scraper",
            dependencies: [
                "SwiftSoup"
            ]
        ),
        .testTarget(
            name: "swift-scraper-tests",
            dependencies: ["swift-scraper"]
        ),
    ]
)
