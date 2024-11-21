namespace VeryLargeTextFile.Sorter.FileSplitting;

class SplittedFileInfoFactory : ISplittedFileInfoFactory
{
    public FileInfo GetFileInfoForSplittedFile(int fileNumber, InputFileSplitterConfig config)
    {
        var filename = Path.Combine(config.SplittedFilesLocation, $"{fileNumber}.not-sorted");
        return new FileInfo(filename);
    }
}
