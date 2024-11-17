using Microsoft.Extensions.Logging;
using System.Text;

namespace VeryLargeTextFile.Generator;

class FileGenerator(IRecordsGenerator recordsGenerator, ILogger<FileGenerator> logger) : IFileGenerator
{
    public async Task GenerateFile(FileInfo fileInfo, long fileSize, int textSize, int textDuplicationFactor)
    {
        var estimatedNumberOfRows = GetEstimatedNumberOfRows(fileSize, textSize);

        logger.LogInformation($"File size to create: {fileSize} B");
        logger.LogInformation($"Number of rows to create: {estimatedNumberOfRows}");

        using var streamWriter = new StreamWriter(fileInfo.FullName, false, Encoding.ASCII, 128 * 1024);
        
        var numberOfSavedRows = 0L;
        while(numberOfSavedRows < estimatedNumberOfRows) 
        {
            var records = recordsGenerator.CreateRecords(textSize, 
                                                        textDuplicationFactor, 
                                                        numberOfRecordsToGenerate: 500);
            foreach (var record in records)
            {
                if (numberOfSavedRows > estimatedNumberOfRows)
                {
                    break;
                }
                await streamWriter.WriteLineAsync(record.ToString());
                ++numberOfSavedRows;
            }
            logger.LogDebug($"{numberOfSavedRows} / {estimatedNumberOfRows} ({(numberOfSavedRows * 100.0 / estimatedNumberOfRows):0.##\\%})");
        }
    }

    static long GetEstimatedNumberOfRows(long fileSize, long textSize)
        => fileSize / (textSize + 2 + 8); //8 - statistically average length of integer string representation
}
