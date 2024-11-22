using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter.SplittedFilesSorting;

class SplittedFileSorter(
    IInputFileStreamFactory inputFileStreamFactory,
    IOutputFileStreamFactory outputFileStreamFactory,
    IComparer<string> comparer,
    ILogger<SplittedFileSorter> logger
    ) : ISplittedFileSorter
{
    public async Task SortFileAndSaveAs(SplittedFile splittedFile, FileInfo outputFileInfo, string[] rowsBuffer, CancellationToken cancellationToken)
    {
        logger.LogDebug("Sorting: {file1} => {file2}", splittedFile.FileInfo.Name, outputFileInfo.Name);

        using var streamReader = new StreamReader(inputFileStreamFactory.CreateInputStream(splittedFile.FileInfo));
        var counter = 0;
        while (!streamReader.EndOfStream)
        {
            rowsBuffer[counter++] = (await streamReader.ReadLineAsync(cancellationToken))!;
        }

        logger.LogDebug("Input file loaded into memory");

        Array.Sort(rowsBuffer, comparer);

        logger.LogDebug($"Rows sorted");


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

        logger.LogDebug("Output file created");

        Array.Clear(rowsBuffer, 0, rowsBuffer.Length);

        logger.LogDebug("Rows buffer zero-ed, ready for new input file");
    }
}
