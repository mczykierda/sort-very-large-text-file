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
    public void WhenReadingBuffer_DealsWithEndOfLine(int fileSize, string text1, string text2, int recordCount1, int recordCount2)
    {
        var sb = new StringBuilder();
        sb.AppendLine("1234567890");
        sb.AppendLine("0987654321");
        sb.Append("abcdefghij");


        var sut = new VeryLargeTextFile.Sorter.FileSplitting.Buffer(new InputFileSplitterConfig(fileSize, "bhjyrtebhyetbeyt"));

        using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        
        sut.ReadFromStream(inputStream);
        var (b1,rc1) = sut.GetBytesAndRecordsCount();
        rc1.Should().Be(recordCount1);
        Encoding.UTF8.GetString(b1).Should().Be(text1);
        sut.Clear();

        sut.ReadFromStream(inputStream);
        var (b2, rc2) = sut.GetBytesAndRecordsCount();
        rc2.Should().Be(recordCount2);
        Encoding.UTF8.GetString(b2).Should().Be(text2);
        sut.Clear();
    }
}
