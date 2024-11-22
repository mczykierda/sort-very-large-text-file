using FluentAssertions;
using System.Text;
using VeryLargeTextFile.Sorter.Merging.SingleRun;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Tests.Sorter.Merging;

public class SaveRowsWithoutLastTrailingEndOfLineTests
{
    [Fact]
    public async Task TestLoopForNotSavingLastEndOfLine()
    {
        using var memoryStream = new MemoryStream();
        using var outputWriter = new StreamWriter(memoryStream);

        var queue = new Queue<string>();

        queue.Enqueue("aaa");
        queue.Enqueue("bbb");
        queue.Enqueue("ccc");

        bool firstLoop = true;
        while (queue.Count > 0)
        {
            var value = queue.Dequeue();

            if (firstLoop)
            {
                firstLoop = false;
            }
            else
            {
                await outputWriter.WriteAsync(Environment.NewLine);
            }
            await outputWriter.WriteAsync(value);
        }

        await outputWriter.FlushAsync();

        var bytes = memoryStream.GetBuffer();
        var actualText = Encoding.UTF8.GetString(bytes, 0, (int)memoryStream.Position);

        actualText.Should().Be("aaa\r\nbbb\r\nccc");
    }
}
