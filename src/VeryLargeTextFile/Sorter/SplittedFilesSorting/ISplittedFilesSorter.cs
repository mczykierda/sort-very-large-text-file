using VeryLargeTextFile.Sorter.FileSplitting;

namespace VeryLargeTextFile.Sorter.SplittedFilesSorting;

public interface ISplittedFilesSorter
{
    IReadOnlyCollection<FileInfo> SortFilesAndSave(SplittingResult splitting, SortConfig config, CancellationToken cancellationToken);
}
