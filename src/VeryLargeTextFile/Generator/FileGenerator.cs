using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Generator;

class FileGenerator(
    IRecordsGenerator recordsGenerator, 
    IOutputFileStreamFactory outputFileStreamFactory,
    ILogger<FileGenerator> logger
    ) 
    : IFileGenerator
{
    public async Task GenerateFile(FileInfo fileInfo, long fileSize, int textSize, int textDuplicationFactor)
    {
        using var executionTimer = new ExecutionTimer(logger, $"Generating file {fileInfo.Name}");

        var estimatedNumberOfRows = GetEstimatedNumberOfRows(fileSize, textSize);

        logger.LogInformation("File size to create: {fileSize} B", fileSize);
        logger.LogInformation("Number of rows to create: {estimatedNumberOfRows}", estimatedNumberOfRows);

        await using var streamWriter = new StreamWriter(outputFileStreamFactory.CreateOutputStream(fileInfo));

        var numberOfSavedRows = 0L;
        while (numberOfSavedRows < estimatedNumberOfRows)
        {
            var records = recordsGenerator.CreateRecords(textSize,
                                                        textDuplicationFactor,
                                                        numberOfRecordsToGenerate: 500);

            foreach (var record in records)
            {
                if (numberOfSavedRows < estimatedNumberOfRows)
                {
                    await streamWriter.WriteAsync($"{record}{Environment.NewLine}");
                    ++numberOfSavedRows;
                }
                else if(numberOfSavedRows == estimatedNumberOfRows)
                {
                    await streamWriter.WriteAsync(record.ToString()); //no newline at the very end of file
                    ++numberOfSavedRows;
                }
                else
                {
                    break;
                }
            }

            logger.LogDebug("{numberOfSavedRows} / {estimatedNumberOfRows} ({percentage:0.##\\%})",
                numberOfSavedRows,
                estimatedNumberOfRows,
                numberOfSavedRows * 100.0 / estimatedNumberOfRows);
        }
    }

    static long GetEstimatedNumberOfRows(long fileSize, long textSize)
    {
        const int separatorSize = 2;
        const int averageCountOfDigitsInInteger = 8; //statistically speaking

        //row format is '<number>. <text>'
        return fileSize / (averageCountOfDigitsInInteger + separatorSize + textSize);
    }
}
