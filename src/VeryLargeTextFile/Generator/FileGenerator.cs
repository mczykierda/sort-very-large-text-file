using Microsoft.Extensions.Logging;

namespace VeryLargeTextFile.Generator;

class FileGenerator(ILogger<FileGenerator> logger) : IFileGenerator
{
    public Task GenerateFile(FileInfo fileInfo, long fileSize, long textSize, int textDuplicationFactor)
    {
        logger.LogDebug("Generated :)");
        return Task.CompletedTask;
    }
}
