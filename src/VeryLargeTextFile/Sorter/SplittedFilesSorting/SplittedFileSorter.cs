using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter.SplittedFilesSorting;

class SplittedFileSorter(
    IInputFileStreamFactory inputFileStreamFactory,
    IOutputFileStreamFactory outputFileStreamFactory,
    IComparer<string> comparer
    ) : ISplittedFileSorter
{
    public async Task SortFileAndSaveAs(SplittedFile splittedFile, FileInfo outputFileInfo, string[] rowsBuffer, CancellationToken cancellationToken)
    {
        
        using var streamReader = new StreamReader(inputFileStreamFactory.CreateInputStream(splittedFile.FileInfo));
        var counter = 0;
        while (!streamReader.EndOfStream)
        {
            rowsBuffer[counter++] = (await streamReader.ReadLineAsync(cancellationToken))!;
        }

        Array.Sort(rowsBuffer, comparer);

        await using var streamWriter = new StreamWriter(outputFileStreamFactory.CreateOutputStream(outputFileInfo));
        bool firstLoop = true;
        foreach (var row in rowsBuffer.Where(x => x is not null))
        {
            if (firstLoop)
            {
                firstLoop = false;
            }
            else
            {
                await streamWriter.WriteAsync(Environment.NewLine);
            }
            await streamWriter.WriteAsync(row);
        }

        Array.Clear(rowsBuffer, 0, rowsBuffer.Length);
    }
}
