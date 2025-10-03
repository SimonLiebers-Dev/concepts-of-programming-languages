//
//  Layout.swift
//  swift-scraper
//
//  Created by Simon Liebers on 03.10.25.
//

import Foundation

struct Layout {
    static func printHeader() {
        let asciiHeader = #"""
            __          __  _     _____
            \ \        / / | |   / ____|
             \ \  /\  / /__| |__| (___   ___ _ __ __ _ _ __   ___ _ __
              \ \/  \/ / _ \ '_ \\___ \ / __| '__/ _' | '_ \ / _ \ '__|
               \  /\  /  __/ |_) |___) | (__| | | (_| | |_) |  __/ |
                \/  \/ \___|_.__/_____/ \___|_|  \__,_| .__/ \___|_|
                                                      | |
                                                      |_|
            """#

        let info = [
            "📦  Parallel Web Scraper (CLI Tool)",
            "👨‍💻  Developer: Simon Liebers",
            "🌐  GitHub: https://github.com/SimonLiebers-Dev/concepts-of-programming-languages",
            "🧠  Built with: Swift 6.1",
        ]

        print(asciiHeader)

        for line in info {
            print(line)
        }
    }

    static func printSeparator() {
        print(String(repeating: "─", count: 100))
    }
}
