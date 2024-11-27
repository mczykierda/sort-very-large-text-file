using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter.SplittedFilesSorting;

class SplittedFilesSorter(
    ISplittedFileSorter sorter, 
    ILogger<SplittedFilesSorter> logger
    ) 
    : ISplittedFilesSorter
{
    public IReadOnlyCollection<FileInfo> SortFilesAndSave(SplittingResult splitting, SortConfig config, CancellationToken cancellationToken)
    {
        using var executionTimer = new ExecutionTimer(logger, $"Sorting step");

        var result = new List<FileInfo>();

        using (var semaphore = new SemaphoreSlim(config.MaxParallelSortingTasks))
        {
            var tasks = new List<Task>(splitting.Files.Count);
            foreach (var splittedFile in splitting.Files)
            {
                semaphore.Wait(cancellationToken);

                var t = Task.Run(async () =>
                {
                    try
                    {
                        var sortedFile = await Sort(splittedFile, splitting.MaxRecordCount, cancellationToken);
                        result.Add(sortedFile);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, 
                cancellationToken);

                tasks.Add(t);
            }

            Task.WaitAll([.. tasks], cancellationToken);
        }

        return result;
    }

    static string GetOutputFileName(FileInfo input)
        => Path.Combine(input.DirectoryName!, input.Name.Replace(input.Extension, ".sorted"));

    async Task<FileInfo> Sort(SplittedFile splittedFile, int maxRecordCount, CancellationToken cancellationToken)
    {
        var outputFile = new FileInfo(GetOutputFileName(splittedFile.FileInfo));
        var rowsBuffer = new string[maxRecordCount];
        await sorter.SortFileAndSaveAs(splittedFile, outputFile, rowsBuffer, cancellationToken);

        splittedFile.FileInfo.Delete();
        logger.LogDebug("{file} deleted", splittedFile.FileInfo.Name);

        return outputFile;
    }
}
