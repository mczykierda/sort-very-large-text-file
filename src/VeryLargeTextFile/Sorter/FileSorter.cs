using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Sorter.Merging;
using VeryLargeTextFile.Sorter.SplittedFilesSorting;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter;

class FileSorter(IInputFileSplitter inputFileSplitter, 
    ISplittedFileSorter fileSorter,
    ISplittedFilesSorter filesSorter,
    ISortedFilesMerger merger,
    IFileOperations fileOperations,
    ILogger<FileSorter> logger) : IFileSorter
{
    public async Task SortFile(FileInfo inputFileInfo, FileInfo outputFileInfo, SortingConfig config, CancellationToken cancellationToken)
    {
        using var executionTimer = new ExecutionTimer(logger, $"Sorting file {inputFileInfo.Name}");

        var splitting = await inputFileSplitter.SplitInputFileIntoSmallerFilesAndSortThem(inputFileInfo, config.Splitting, cancellationToken);
        if(splitting.Files.Count == 1)
        {
            await SortSingleFileInMemoryAndSaveAsFinalFile(splitting.Files.Single(), outputFileInfo, cancellationToken);
            return;
        }
        var sortedFiles = await filesSorter.SortFilesAndSave(splitting, cancellationToken);
        var mergedFileInfo = await merger.MergeFiles(sortedFiles, config.Merging, cancellationToken);

        fileOperations.Move(mergedFileInfo, outputFileInfo, config.OverwriteOutputFile);
        logger.LogDebug($"Final merged file: {outputFileInfo.FullName}, size: {outputFileInfo.Length}");
    }

    async Task SortSingleFileInMemoryAndSaveAsFinalFile(SplittedFile splittedFile, FileInfo outputFileInfo, CancellationToken cancellationToken)
    {
        logger.LogDebug("Single file detected after splitting step: sort it and save as output file");
        var rowsBuffer = new string[splittedFile.RecordCount];
        await fileSorter.SortFileAndSaveAs(splittedFile, outputFileInfo, rowsBuffer, cancellationToken);
        logger.LogDebug($"Final file: {outputFileInfo.FullName}, size: {outputFileInfo.Length}");
    }
}
