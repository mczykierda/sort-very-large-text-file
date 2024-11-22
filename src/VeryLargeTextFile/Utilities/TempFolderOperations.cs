using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Sorter.FileSplitting;

namespace VeryLargeTextFile.Utilities;

class TempFolderOperations(ILogger<TempFolderOperations> logger) : ITempFolderOperations,
    IDisposable
{
    InputFileSplitterConfig? _config;

    public void Create(InputFileSplitterConfig config)
    {
        _config = config;
        Directory.CreateDirectory(_config.SplittedFilesLocation);

        logger.LogDebug($"Temp folder created: {_config.SplittedFilesLocation}");
    }

    public FileInfo GetFileInfoForSplittedFile(int fileNumber)
    {
        var filename = Path.Combine(_config!.SplittedFilesLocation, $"{fileNumber}.not-sorted");
        return new FileInfo(filename);
    }

    public FileInfo GetFileInfoForMergedFile(int mergeRunNumber)
    {
        var filename = Path.Combine(_config!.SplittedFilesLocation, $"merge-{mergeRunNumber}.sorted");
        return new FileInfo(filename);
    }

    public void Dispose()
    {
        Directory.Delete(_config!.SplittedFilesLocation, true);
        logger.LogDebug($"Temp folder deleted: {_config.SplittedFilesLocation}");
    }
}
