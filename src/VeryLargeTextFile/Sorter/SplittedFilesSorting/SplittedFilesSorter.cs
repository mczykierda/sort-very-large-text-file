using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Sorter.FileSplitting;

namespace VeryLargeTextFile.Sorter.SplittedFilesSorting;

class SplittedFilesSorter(ISplittedFileSorter sorter, ILogger<SplittedFilesSorter> logger) : ISplittedFilesSorter
{
    public async Task<IReadOnlyCollection<FileInfo>> SortFilesAndSave(SplittingResult splitting, CancellationToken cancellationToken)
    {
        var result = new List<FileInfo>();
        var rowsBuffer = new string[splitting.MaxRecordCount];
        logger.LogDebug($"InMemory sorting buffer size: {splitting.MaxRecordCount}");

        foreach (var splittedFile in splitting.Files)
        {
            logger.LogDebug($"Sorting {splittedFile.FileInfo.Name}");
            
            var outputFile = new FileInfo(GetOutputFileName(splittedFile.FileInfo));
            await sorter.SortFileAndSaveAs(splittedFile, outputFile, rowsBuffer, cancellationToken);

            splittedFile.FileInfo.Delete();
            
            logger.LogDebug($"{splittedFile.FileInfo.Name} deleted");
            result.Add(outputFile);
        }
        return result;
    }

    string GetOutputFileName(FileInfo input)
        => Path.Combine(input.DirectoryName!, input.Name.Replace(input.Extension, ".sorted"));
}