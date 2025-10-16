using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using WebScraper.Core.Fetcher;

namespace WebScraper.Core.Tests.Fetcher;

[TestFixture]
public class HtmlFetcherTests
{
    private Mock<ILogger<HtmlFetcher>> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = new Mock<ILogger<HtmlFetcher>>();
    }

    private static HttpClient CreateHttpClient(HttpStatusCode status, string content = "")
    {
        var handler = new FakeHttpMessageHandler(status, content);
        return new HttpClient(handler);
    }

    [Test]
    public async Task FetchAsync_ShouldReturnHtml_WhenRequestIsSuccessful()
    {
        // Arrange
        const string url = "https://example.com";
        const string html = "<html><body>Hello World</body></html>";

        var httpClient = CreateHttpClient(HttpStatusCode.OK, html);
        var fetcher = new HtmlFetcher(httpClient, _logger.Object);

        // Act
        var result = await fetcher.FetchAsync(url);

        // Assert
        Assert.That(result, Is.EqualTo(html));
    }

    [Test]
    public void FetchAsync_ShouldThrow_WhenUrlIsEmpty()
    {
        // Arrange
        var httpClient = CreateHttpClient(HttpStatusCode.OK);
        var fetcher = new HtmlFetcher(httpClient, _logger.Object);

        // Act + Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await fetcher.FetchAsync(""));
        Assert.That(ex!.Message, Does.Contain("URL cannot be null or empty."));
    }

    [Test]
    public void FetchAsync_ShouldThrow_WhenStatusCodeIsNotSuccess()
    {
        // Arrange
        const string url = "https://example.com/error";
        var httpClient = CreateHttpClient(HttpStatusCode.NotFound);
        var fetcher = new HtmlFetcher(httpClient, _logger.Object);

        // Act + Assert
        var ex = Assert.ThrowsAsync<HttpRequestException>(async () => await fetcher.FetchAsync(url));
        Assert.That(ex!.Message, Does.Contain("404 Not Found"));
    }

    [Test]
    public void SetUserAgent_ShouldApplyCustomUserAgent()
    {
        // Arrange
        const string userAgent = "Custom User Agent";
        const string url = "https://example.com";

        var handler = new CapturingHttpMessageHandler();
        var httpClient = new HttpClient(handler);
        var fetcher = new HtmlFetcher(httpClient, _logger.Object);

        fetcher.SetUserAgent(userAgent);

        // Act
        Assert.ThrowsAsync<HttpRequestException>(async () => await fetcher.FetchAsync(url));

        // Assert
        Assert.That(handler.LastRequest!.Headers.UserAgent.ToString(), Does.Contain(userAgent));
    }

    [Test]
    public void FetchAsync_ShouldPropagateCancellation()
    {
        // Arrange
        var handler = new DelayedHttpMessageHandler(TimeSpan.FromSeconds(5));
        var httpClient = new HttpClient(handler);
        var fetcher = new HtmlFetcher(httpClient, _logger.Object);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act + Assert
        Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await fetcher.FetchAsync("https://example.com", cts.Token));
    }

    #region Helper functions

    private class FakeHttpMessageHandler(HttpStatusCode status, string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent(content)
            };
            return Task.FromResult(response);
        }
    }

    private class CapturingHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            // Always fail intentionally, so FetchAsync throws (we just need to inspect headers)
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
        }
    }

    private class DelayedHttpMessageHandler(TimeSpan delay) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await Task.Delay(delay, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }

    #endregion
}