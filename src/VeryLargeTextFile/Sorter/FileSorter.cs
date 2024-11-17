
using Microsoft.Extensions.Logging;

namespace VeryLargeTextFile.Sorter;

class FileSorter(ILogger<FileSorter> logger) : IFileSorter
{
    public Task SortFile(FileInfo inputFileInfo, FileInfo outputFileInfo)
    {
        logger.LogDebug("Sorted :)");
        return Task.CompletedTask;
    }
}
