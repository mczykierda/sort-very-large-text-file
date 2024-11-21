namespace VeryLargeTextFile.Utilities;

public interface IOutputFileStreamFactory
{
    Stream CreateOutputStream(FileInfo fileInfo);
}
