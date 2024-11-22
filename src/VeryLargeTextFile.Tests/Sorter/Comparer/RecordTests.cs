using FluentAssertions;
using VeryLargeTextFile.Sorter.Comparer;

namespace VeryLargeTextFile.Tests.Sorter.Comparer;

public class RecordTests
{
    [Fact]
    public void WhenParsingGoodValue_ShouldNotThrowException()
    {
        var text = "gtewcgrewc";
        var number = 432432;
        var record = $"{number}. {text}";
        var result = VeryLargeTextFile.Sorter.Comparer.Record.Parse(record);

        var actualText = new string(result.Text);
        actualText.Should().Be(text);
        result.Number.Should().Be(number);
    }

    [Fact]
    public void WhenParsingValueWithoutDotSeparator_ShouldThrowException()
    {
        var text = "gtewcgrewc";
        var number = 432432;
        var record = $"{number} {text}";

        var exception = Xunit.Record.Exception(() => VeryLargeTextFile.Sorter.Comparer.Record.Parse(record));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidRecordFormatException>();
        (exception as InvalidRecordFormatException).Record.Should().Be(record);

    }
}
