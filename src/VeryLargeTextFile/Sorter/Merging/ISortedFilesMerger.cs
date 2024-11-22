namespace VeryLargeTextFile.Sorter.Merging;

public interface ISortedFilesMerger
{
    Task<FileInfo> MergeFiles(IReadOnlyCollection<FileInfo> initialSortedFiles, MergeConfig config, CancellationToken cancellationToken);
}
