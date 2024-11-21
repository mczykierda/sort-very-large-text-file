using Microsoft.Extensions.Logging;

namespace VeryLargeTextFile.Sorter.FileSplitting;

class InputFileSplitterConfigurator(ILogger<InputFileSplitterConfigurator> logger) : IInputFileSplitterConfigurator,
    IDisposable
{
    InputFileSplitterConfig _config;

    public void Configure(InputFileSplitterConfig config)
    {
        _config = config;
        Directory.CreateDirectory(_config.SplittedFilesLocation);

        logger.LogDebug($"Temp folder created: {_config.SplittedFilesLocation}");
    }

    public void Dispose()
    {
        Directory.Delete(_config.SplittedFilesLocation, true);
        logger.LogDebug($"Temp folder deleted: {_config.SplittedFilesLocation}");
    }
}
