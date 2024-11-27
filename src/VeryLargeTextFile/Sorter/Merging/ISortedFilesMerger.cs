namespace VeryLargeTextFile.Sorter.Merging;

public interface ISortedFilesMerger
{
    FileInfo MergeFiles(IReadOnlyCollection<FileInfo> initialSortedFiles, MergeConfig config, CancellationToken cancellationToken);
}
