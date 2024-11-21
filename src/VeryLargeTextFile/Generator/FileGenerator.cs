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

        await using var streamWriter = CreateStreamWriter(fileInfo);

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

            logger.LogDebug($"{numberOfSavedRows} / {estimatedNumberOfRows} ({(numberOfSavedRows * 100.0 / estimatedNumberOfRows):0.##\\%})");
        }
    }

    static long GetEstimatedNumberOfRows(long fileSize, long textSize)
    {
        const int separatorSize = 2;
        const int averageCountOfDigitsInInteger = 8; //statistically speaking

        //row format is '<number>. <text>'
        return fileSize / (averageCountOfDigitsInInteger + separatorSize + textSize);
    }

    static StreamWriter CreateStreamWriter(FileInfo fileInfo)
    {
        var bufferSize = 128 * 1024;
        return new StreamWriter(fileInfo.FullName, false, Encoding.UTF8, bufferSize);
    }
}
