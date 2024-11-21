
using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Sorter.SplittedFilesSorting;

namespace VeryLargeTextFile.Sorter;

class FileSorter(IInputFileSplitter inputFileSplitter, 
    ISplittedFileSorter fileSorter,
    ISplittedFilesSorter filesSorter,
    ILogger<FileSorter> logger) : IFileSorter
{
    public async Task SortFile(FileInfo inputFileInfo, FileInfo outputFileInfo, SortingConfig config, CancellationToken cancellationToken)
    {
        var splitting = await inputFileSplitter.SplitInputFileIntoSmallerFilesAndSortThem(inputFileInfo, config.Splitting, cancellationToken);
        if(splitting.Files.Count() == 1)
        {
            await SortSingleFileInMemoryAndSaveAsFinalFile(splitting.Files.Single(), outputFileInfo, cancellationToken);
            return;
        }
        var sortedFiles = await filesSorter.SortFilesAndSave(splitting, cancellationToken);


        logger.LogDebug("Sorted :)");
    }

    async Task SortSingleFileInMemoryAndSaveAsFinalFile(SplittedFile splittedFile, FileInfo outputFileInfo, CancellationToken cancellationToken)
    {
        logger.LogDebug("Single file detected: sort it and save as output file");
        var rowsBuffer = new string[splittedFile.RecordCount];
        await fileSorter.SortFileAndSaveAs(splittedFile, outputFileInfo, rowsBuffer, cancellationToken);
    }
}
