using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter.FileSplitting;

class InputFileSplitter(
    IInputFileStreamFactory inputFileStreamFactory,
    ITempFolderOperations tempFolder,
    IOutputFileStreamFactory outputFileStreamFactory,
    ILogger<InputFileSplitter> logger
    ) : IInputFileSplitter
{
    public async Task<SplittingResult> SplitInputFileIntoSmallerFilesAndSortThem(FileInfo inputFileInfo, InputFileSplitterConfig config, CancellationToken cancellationToken)
    {
        using var executionTimer = new ExecutionTimer(logger, $"Splitting step");

        tempFolder.Create(config);

        var result = new List<SplittedFile>();
        var buffer = new Buffer(config);
        var currentFileNumber = 0;

        await using var inputStream = inputFileStreamFactory.CreateInputStream(inputFileInfo);

        logger.LogDebug("Creating splitted files...");

        while (!inputStream.EndOfStream())
        {
            cancellationToken.ThrowIfCancellationRequested();

            buffer.ReadFromStream(inputStream);

            var splittedFileInfo = tempFolder.GetFileInfoForSplittedFile(currentFileNumber);
            await using var outputStream = outputFileStreamFactory.CreateOutputStream(splittedFileInfo);

            await buffer.SaveToStream(outputStream, cancellationToken);

            var splittedFile = new SplittedFile(splittedFileInfo, buffer.RecordsCount);
            logger.LogDebug("Splitted file created: {file}, size: {size}, records: {count}",
                splittedFile.FileInfo.Name,
                splittedFile.FileInfo.Length,
                splittedFile.RecordCount);

            result.Add(splittedFile);

            buffer.Clear();
            currentFileNumber++;
        }
        var splittingResult = new SplittingResult(result);
        logger.LogDebug("Splitting completed, max number of rows detected: {count}", splittingResult.MaxRecordCount);
        
        return splittingResult;
    }
}
