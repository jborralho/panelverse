using FluentAssertions;
using Panelverse.Core.Sorting;

namespace Panelverse.Core.Tests;

public class NaturalSortTests
{
    [Theory]
    [InlineData("1", "2")]
    [InlineData("2", "10")]
    [InlineData("page2.jpg", "page10.jpg")]
    [InlineData("Page2.jpg", "page10.JPG")]
    [InlineData("001", "02")]
    public void Should_order_numbers_naturally(string smaller, string larger)
    {
        NaturalSort.Compare(smaller, larger).Should().BeLessThan(0);
        NaturalSort.Compare(larger, smaller).Should().BeGreaterThan(0);
    }

    [Fact]
    public void Equal_numbers_with_different_zero_padding_prefers_shorter()
    {
        NaturalSort.Compare("02", "002").Should().BeLessThan(0);
    }

    [Fact]
    public void Null_safety()
    {
        NaturalSort.Compare(null, "a").Should().BeLessThan(0);
        NaturalSort.Compare("a", null).Should().BeGreaterThan(0);
        NaturalSort.Compare(null, null).Should().Be(0);
    }
}


