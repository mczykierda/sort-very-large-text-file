using FluentAssertions;
using System.Text;
using VeryLargeTextFile.Sorter.FileSplitting;

namespace VeryLargeTextFile.Tests.Sorter.FileSplitting;

public class BufferTests
{
    [Theory]
    [InlineData(2, "1234567890", "0987654321", 1, 1)]
    [InlineData(7, "1234567890", "0987654321", 1, 1)]
    [InlineData(9, "1234567890", "0987654321", 1, 1)]
    [InlineData(10, "1234567890", "0987654321", 1, 1)]
    [InlineData(11, "1234567890", "0987654321", 1, 1)]
    [InlineData(12, "1234567890", "0987654321", 1, 1)]
    [InlineData(13, "1234567890\r\n0987654321", "abcdefghij", 2, 1)]
    [InlineData(14, "1234567890\r\n0987654321", "abcdefghij", 2, 1)]
    [InlineData(20, "1234567890\r\n0987654321", "abcdefghij", 2, 1)]
    [InlineData(21, "1234567890\r\n0987654321", "abcdefghij", 2, 1)]
    [InlineData(22, "1234567890\r\n0987654321", "abcdefghij", 2, 1)]
    [InlineData(23, "1234567890\r\n0987654321", "abcdefghij", 2, 1)]
    [InlineData(24, "1234567890\r\n0987654321", "abcdefghij", 2, 1)]
    [InlineData(25, "1234567890\r\n0987654321\r\nabcdefghij", "", 3, 0)]
    [InlineData(26, "1234567890\r\n0987654321\r\nabcdefghij", "", 3, 0)]
    [InlineData(100, "1234567890\r\n0987654321\r\nabcdefghij", "", 3, 0)]
    public async Task WhenReadingBuffer_DealsWithEndOfLine(int fileSize, string text1, string text2, int recordCount1, int recordCount2)
    {
        var sb = new StringBuilder();
        sb.AppendLine("1234567890");
        sb.AppendLine("0987654321");
        sb.AppendLine("abcdefghij");


        var sut = new VeryLargeTextFile.Sorter.FileSplitting.Buffer(new InputFileSplitterConfig(fileSize, "gecge"));

        using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        sut.ReadFromStream(inputStream);

        using var outputStream1 = new MemoryStream();
        await sut.SaveToStream(outputStream1, CancellationToken.None);

        sut.RecordsCount.Should().Be(recordCount1);
        sut.Clear();

        sut.ReadFromStream(inputStream);

        using var outputStream2 = new MemoryStream();
        await sut.SaveToStream(outputStream2, CancellationToken.None);

        sut.RecordsCount.Should().Be(recordCount2);
        sut.Clear();

        var bytes1 = outputStream1.GetBuffer();
        var actualText1 = Encoding.UTF8.GetString(bytes1, 0, (int)outputStream1.Position);

        var bytes2 = outputStream2.GetBuffer();
        var actualText2 = Encoding.UTF8.GetString(bytes2, 0, (int)outputStream2.Position);

        actualText1.Should().Be(text1);
        actualText2.Should().Be(text2);
    }
}
