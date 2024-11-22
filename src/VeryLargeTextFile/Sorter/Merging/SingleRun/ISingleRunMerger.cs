namespace VeryLargeTextFile.Sorter.Merging.SingleRun;

public interface ISingleRunMerger
{
    Task<FileInfo> MergeFiles(IEnumerable<FileInfo> files, int mergeRunCounter, CancellationToken cancellationToken);
}
