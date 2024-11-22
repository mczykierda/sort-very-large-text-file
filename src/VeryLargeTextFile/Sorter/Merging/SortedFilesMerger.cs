using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Sorter.Merging.SingleRun;

namespace VeryLargeTextFile.Sorter.Merging;

class SortedFilesMerger(
    ISingleRunMerger merger,
    ILogger<SortedFilesMerger> logger
    ) : ISortedFilesMerger
{
    public async Task<FileInfo> MergeFiles(IReadOnlyCollection<FileInfo> initialSortedFiles, MergeConfig config, CancellationToken cancellationToken)
    {
        logger.LogDebug($"Merging {initialSortedFiles.Count} files...");
        var queue = new SortedFilesQueue(initialSortedFiles, config);

        var mergeRunCounter = 0;
        while (queue.HasFilesToMerge)
        {
            var filesToMerge = queue.GetNextBatchOfFilesToMerge();
            var mergedFile = await merger.MergeFiles(filesToMerge, mergeRunCounter, cancellationToken);

            if(!queue.HasFilesToMerge)
            {
                logger.LogDebug($"No more files to merge. Final file: {mergedFile.Name}");
                return mergedFile;
            }
            queue.AddMergedFile(mergedFile);
            mergeRunCounter++;
            logger.LogDebug($"There are other files to merge with, adding {mergedFile.Name} to queue of files to merge");
        }

        throw new Exception("Should never happen!");
    }
}
