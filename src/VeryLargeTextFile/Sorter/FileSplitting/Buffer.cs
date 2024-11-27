namespace VeryLargeTextFile.Sorter.FileSplitting;

public class Buffer(InputFileSplitterConfig config)
{
    const byte EndOfLine = (byte)'\n'; //windows: \r\n, linux: \n, ultimately it's \n which we are looking at
    static readonly byte[] PossibleEndOfLineCharacters = [(byte)'\n', (byte)'\r'];

    readonly byte[] _bytes = new byte[config.FileSize];
    readonly List<byte> _lastRecordBytes = new(2 * 1024);

    public int RecordsCount { get; private set; }
    int _bytesCount = 0;

    public void ReadFromStream(Stream stream)
    {
        while (_bytesCount < config.FileSize)
        {
            var value = stream.ReadByte();
            if (value == -1)
            {
                break;
            }

            var b = (byte)value;
            _bytes[_bytesCount] = b;
            _bytesCount++;
            if (b == EndOfLine)
            {
                RecordsCount++;
            }
        }

        if(_bytesCount == 0)
        {
            return;
        }

        var lastRecordByte = _bytes[_bytesCount - 1];

        //excellent, we have full line in the buffer, clear the finishing end of line if it's there
        while (PossibleEndOfLineCharacters.Contains(_bytes[_bytesCount - 1]))
        {
            _bytesCount--;
        }

        //load some more bytes until the end of current line
        while (lastRecordByte != EndOfLine)
        {
            var value = stream.ReadByte();
            if (value == -1)
            {
                break;
            }
            lastRecordByte = (byte)value;
            _lastRecordBytes.Add(lastRecordByte);
        }
        if (HasAdditionalBytesFinishingTheLastRecord)
        {
            RecordsCount++;
            //we may have end of line at the end, remove it
            while (HasAdditionalBytesFinishingTheLastRecord
                && PossibleEndOfLineCharacters.Contains(_lastRecordBytes[^1]))
            {
                _lastRecordBytes.RemoveAt(_lastRecordBytes.Count - 1);
            }
        }
    }

    bool HasAdditionalBytesFinishingTheLastRecord => _lastRecordBytes.Count > 0;

    public async Task SaveToStream(Stream stream, CancellationToken cancellationToken)
    {
        await stream.WriteAsync(_bytes.AsMemory(0, _bytesCount), cancellationToken);
        if (HasAdditionalBytesFinishingTheLastRecord)
        {
            await stream.WriteAsync(_lastRecordBytes.ToArray().AsMemory(), cancellationToken);
        }
    }

    public void Clear()
    {
        _lastRecordBytes.Clear();
        RecordsCount = 0;
        _bytesCount = 0;
    }
}
