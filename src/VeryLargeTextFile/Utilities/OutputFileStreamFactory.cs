namespace VeryLargeTextFile.Utilities;

class OutputFileStreamFactory : IOutputFileStreamFactory
{
    public Stream CreateOutputStream(FileInfo fileInfo)
    {
        const int bufferSize = 64 * 1024;
        return new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);
    }
}
