using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Sorter.Merging;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter;

class FileSorter(IInputFileSplitter inputFileSplitter, 
    ISortedFilesMerger merger,
    IFileOperations fileOperations,
    ILogger<FileSorter> logger) : IFileSorter
{
    public async Task SortFile(FileInfo inputFileInfo, FileInfo outputFileInfo, FileSortingConfig config, CancellationToken cancellationToken)
    {
        using var executionTimer = new ExecutionTimer(logger, $"Sorting file {inputFileInfo.Name}");

        var splittedFiles = await inputFileSplitter.SplitInputFileIntoSmallerFilesAndSortThem(inputFileInfo, config.Splitting, cancellationToken);
        if(splittedFiles.Count == 1)
        {
            fileOperations.Move(splittedFiles.Single(), outputFileInfo, config.OverwriteOutputFile);
            return;
        }
        var mergedFileInfo = merger.MergeFiles(splittedFiles, config.Merging, cancellationToken);

        fileOperations.Move(mergedFileInfo, outputFileInfo, config.OverwriteOutputFile);
        logger.LogDebug("Final merged file: {file}, size: {size}", outputFileInfo.FullName, outputFileInfo.Length);
    }
}
