using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter.Merging.SingleRun;

public class File(
    FileInfo fileInfo, 
    IInputFileStreamFactory inputFileStreamFactory, 
    IFileOperations fileOperations
    )
    : IDisposable
{
    readonly StreamReader _streamReader = new(inputFileStreamFactory.CreateInputStream(fileInfo));
    public FileInfo FileInfo => fileInfo;

    public bool IsFullyProcessed => _streamReader.EndOfStream;

    public async Task MoveNext()
    {
        if (!IsFullyProcessed)
        {
            Value = await _streamReader.ReadLineAsync();
        }
    }

    public string? Value { get; private set; }

    public void Dispose()
    {
        ((IDisposable)_streamReader).Dispose();
        fileOperations.Delete(fileInfo);
    }
}
