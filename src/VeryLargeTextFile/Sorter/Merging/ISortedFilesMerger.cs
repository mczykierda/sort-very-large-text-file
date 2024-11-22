namespace VeryLargeTextFile.Sorter.Merging;

public interface ISortedFilesMerger
{
    Task<FileInfo> MergeFiles(IEnumerable<FileInfo> initialSortedFiles, MergeConfig config, CancellationToken cancellationToken);
}
