using VeryLargeTextFile.Sorter.Merging.SingleRun;

namespace VeryLargeTextFile.Sorter.Merging;

class SortedFilesMerger(ISingleRunMerger merger) : ISortedFilesMerger
{
    public async Task<FileInfo> MergeFiles(IEnumerable<FileInfo> initialSortedFiles, MergeConfig config, CancellationToken cancellationToken)
    {
        var queue = new SortedFilesQueue(initialSortedFiles, config);

        var mergeRunCounter = 0;
        while (queue.HasFilesToMerge)
        {
            var filesToMerge = queue.GetNextBatchOfFilesToMerge();
            var mergedFile = await merger.MergeFiles(filesToMerge, mergeRunCounter, cancellationToken);
            if(!queue.HasFilesToMerge)
            {
                return mergedFile;
            }
            queue.AddMergedFile(mergedFile);
            mergeRunCounter++;
        }
        throw new Exception("Should never happen!");
    }
}
