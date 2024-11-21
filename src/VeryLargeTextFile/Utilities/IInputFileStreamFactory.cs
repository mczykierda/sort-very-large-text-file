namespace VeryLargeTextFile.Utilities;

public interface IInputFileStreamFactory
{
    Stream CreateInputStream(FileInfo fileInfo);
}
