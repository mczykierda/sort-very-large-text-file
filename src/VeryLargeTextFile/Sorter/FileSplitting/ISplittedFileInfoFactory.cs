namespace VeryLargeTextFile.Sorter.FileSplitting;

public interface ISplittedFileInfoFactory
{
    FileInfo GetFileInfoForSplittedFile(int fileNumber, InputFileSplitterConfig config);
}
