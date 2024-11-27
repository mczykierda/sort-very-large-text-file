using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter.FileSplitting;

class InputFileSplitter(
    IInputFileStreamFactory inputFileStreamFactory,
    ITempFolderOperations tempFolder,
    IOutputFileStreamFactory outputFileStreamFactory,
    IComparer<string> comparer,
    ILogger<InputFileSplitter> logger
    ) : IInputFileSplitter
{
    public async Task<IReadOnlyCollection<FileInfo>> SplitInputFileIntoSmallerFilesAndSortThem(FileInfo inputFileInfo, InputFileSplitterConfig config, CancellationToken cancellationToken)
    {
        using var executionTimer = new ExecutionTimer(logger, $"Splitting step");

        tempFolder.Create(config);

        var result = new ConcurrentBag<FileInfo>();
        var buffer = new Buffer(config);
        var currentFileNumber = 0;
        var tasks = new List<Task>();

        await using var inputStream = inputFileStreamFactory.CreateInputStream(inputFileInfo);

        logger.LogDebug("Creating splitted files...");

        while (!inputStream.EndOfStream())
        {
            cancellationToken.ThrowIfCancellationRequested();

            buffer.ReadFromStream(inputStream);
            var (bytes, recordsCount) = buffer.GetBytesAndRecordsCount();

            var t = Task.Run(async () =>
                            {
                                var splittedFileInfo = await SortAndSave(bytes, recordsCount, currentFileNumber, cancellationToken);
                                result.Add(splittedFileInfo);
                            },
                            cancellationToken);

            tasks.Add(t);

            buffer.Clear();
            currentFileNumber++;
        }

        Task.WaitAll([.. tasks], cancellationToken);

        return result;
    }

    async Task<FileInfo> SortAndSave(byte[] bytes, int recordsCount, int currentFileNumber, CancellationToken cancellationToken)
    {
        logger.LogDebug("Sorting file number {fileNumber}", currentFileNumber);

        using var memoryStream = new MemoryStream(bytes);
        using var streamReader = new StreamReader(memoryStream);

        var rows = new string[recordsCount];

        var counter = 0;
        while (!streamReader.EndOfStream)
        {
            rows[counter++] = (await streamReader.ReadLineAsync(cancellationToken))!;
        }

        Array.Sort(rows, comparer);

        var splittedFileInfo = tempFolder.GetFileInfoForSplittedFile(currentFileNumber);
        await using var streamWriter = new StreamWriter(outputFileStreamFactory.CreateOutputStream(splittedFileInfo));

        logger.LogDebug("Saving to file {file}", splittedFileInfo.Name);
        bool firstLoop = true;
        foreach (var row in rows.Where(x => x is not null))
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

        logger.LogDebug("Splitted file created: {file}, size: {size}, records: {records}", splittedFileInfo.Name, splittedFileInfo.Length, recordsCount);
        return splittedFileInfo;
    }
}
