
using VeryLargeTextFile.Sorter.FileSplitting;

namespace VeryLargeTextFile.Sorter.SplittedFilesSorting;

public interface ISplittedFileSorter
{
    Task SortFileAndSaveAs(SplittedFile splittedFile, FileInfo outputFileInfo, string[] rowsBuffer, CancellationToken cancellationToken);
}
