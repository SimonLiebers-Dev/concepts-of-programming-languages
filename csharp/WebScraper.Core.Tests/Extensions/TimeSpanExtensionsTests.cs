using System.Globalization;
using WebScraper.Core.Extensions;

namespace WebScraper.Core.Tests.Extensions;

[TestFixture]
public class TimeSpanExtensionsTests
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    [Test]
    public void ToFormattedString_ShouldFormatMilliseconds_WhenUnderOneSecond()
    {
        var ts = TimeSpan.FromMilliseconds(350);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("350ms"));
    }

    [Test]
    public void ToFormattedString_ShouldRoundMillisecondsCorrectly()
    {
        var ts = TimeSpan.FromMilliseconds(752.6);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("753ms"));
    }

    [Test]
    public void ToFormattedString_ShouldFormatSeconds_WhenUnderOneMinute()
    {
        var ts = TimeSpan.FromSeconds(12.34);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("12.3s"));
    }

    [Test]
    public void ToFormattedString_ShouldHandleEdgeOfOneSecondBoundary()
    {
        var ts = TimeSpan.FromMilliseconds(999);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("999ms"));
    }

    [Test]
    public void ToFormattedString_ShouldFormatMinutesAndSeconds_WhenUnderOneHour()
    {
        var ts = new TimeSpan(0, 2, 5);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("2m 05s"));
    }

    [Test]
    public void ToFormattedString_ShouldHandle59MinutesCorrectly()
    {
        var ts = new TimeSpan(0, 59, 59);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("59m 59s"));
    }

    [Test]
    public void ToFormattedString_ShouldFormatHoursAndMinutes_WhenUnderOneDay()
    {
        var ts = new TimeSpan(3, 42, 0);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("3h 42m"));
    }

    [Test]
    public void ToFormattedString_ShouldHandle23HoursCorrectly()
    {
        var ts = new TimeSpan(23, 59, 0);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("23h 59m"));
    }

    [Test]
    public void ToFormattedString_ShouldFormatDaysAndHours_WhenOneDayOrMore()
    {
        var ts = new TimeSpan(1, 4, 0, 0);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("1d 04h"));
    }

    [Test]
    public void ToFormattedString_ShouldHandleMultipleDays()
    {
        var ts = new TimeSpan(2, 12, 30, 0);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("2d 12h"));
    }

    [Test]
    public void ToFormattedString_ShouldHandleZero()
    {
        var ts = TimeSpan.Zero;

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("0ms"));
    }

    [Test]
    public void ToFormattedString_ShouldHandleExactlyOneSecond()
    {
        var ts = TimeSpan.FromSeconds(1);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("1.0s"));
    }

    [Test]
    public void ToFormattedString_ShouldHandleExactlyOneMinute()
    {
        var ts = TimeSpan.FromMinutes(1);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("1m 00s"));
    }

    [Test]
    public void ToFormattedString_ShouldHandleExactlyOneHour()
    {
        var ts = TimeSpan.FromHours(1);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("1h 00m"));
    }

    [Test]
    public void ToFormattedString_ShouldHandleExactlyOneDay()
    {
        var ts = TimeSpan.FromDays(1);

        var result = ts.ToFormattedString(Invariant);

        Assert.That(result, Is.EqualTo("1d 00h"));
    }
}