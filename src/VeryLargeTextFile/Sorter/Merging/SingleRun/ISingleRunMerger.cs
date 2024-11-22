namespace VeryLargeTextFile.Sorter.Merging.SingleRun;

public interface ISingleRunMerger
{
    Task<FileInfo> MergeFiles(IReadOnlyCollection<FileInfo> files, int mergeRunCounter, CancellationToken cancellationToken);
}
