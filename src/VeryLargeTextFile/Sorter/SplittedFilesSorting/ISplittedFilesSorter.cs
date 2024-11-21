using VeryLargeTextFile.Sorter.FileSplitting;

namespace VeryLargeTextFile.Sorter.SplittedFilesSorting;

public interface ISplittedFilesSorter
{
    Task<IReadOnlyCollection<FileInfo>> SortFilesAndSave(SplittingResult splitting, CancellationToken cancellationToken);
}
