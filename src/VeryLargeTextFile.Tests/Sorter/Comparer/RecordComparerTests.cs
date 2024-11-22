using FluentAssertions;
using VeryLargeTextFile.Sorter.Comparer;

namespace VeryLargeTextFile.Tests.Sorter.Comparer;

public class RecordComparerTests
{
    [Theory]
    [InlineData(null, null, 0)]
    [InlineData(null, "abc", -1)]
    [InlineData("abc", null, 1)]
    [InlineData("123. def", "123. abc", 3)]
    [InlineData("123. abc", "123. def", -3)]
    [InlineData("123. abc", "123. abc", 0)]
    [InlineData("456. abc", "123. abc", 1)]
    [InlineData("123. abc", "456. abc", -1)]
    public void WhenComparing_ShouldHaveCorrectResult(string? x, string? y, int expectedResult)
    {
        var sut = new RecordComparer();

        var result = sut.Compare(x, y);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123 abc", "123. abc")]
    [InlineData("123. abc", "123 abc")]
    public void WhenComparingWithIncorrectValue_ShouldthrowException(string? x, string? y)
    {
        var sut = new RecordComparer();

        var exception = Xunit.Record.Exception(() => sut.Compare(x, y));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidRecordFormatException>();
        (exception as InvalidRecordFormatException).Record.Should().Be("123 abc");
    }
}