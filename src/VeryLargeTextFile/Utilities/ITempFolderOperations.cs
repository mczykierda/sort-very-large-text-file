using VeryLargeTextFile.Sorter.FileSplitting;

namespace VeryLargeTextFile.Utilities;

public interface ITempFolderOperations
{
    void Create(InputFileSplitterConfig config);
    FileInfo GetFileInfoForSplittedFile(int fileNumber);
    FileInfo GetFileInfoForMergedFile(int mergeRunNumber);
}
