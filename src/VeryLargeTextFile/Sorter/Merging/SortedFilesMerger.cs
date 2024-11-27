using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Sorter.Merging.SingleRun;
using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter.Merging;

class SortedFilesMerger(
    ISingleRunMerger merger,
    ILogger<SortedFilesMerger> logger
    ) : ISortedFilesMerger
{
    public FileInfo MergeFiles(IReadOnlyCollection<FileInfo> initialSortedFiles, MergeConfig config, CancellationToken cancellationToken)
    {
        using var executionTimer = new ExecutionTimer(logger, $"Merging step");

        logger.LogDebug("Merging {count} files...", initialSortedFiles.Count);
        var queue = new SortedFilesQueue(initialSortedFiles, config);
        var mergeRunCounter = 0;

        while (queue.HasFilesToMerge)
        {
            if (queue.Count == 1)
            {
                var files = queue.GetNextBatchOfFilesToMerge();
                logger.LogDebug("No more files to merge. Final file: {file}", files.Single().Name);
                return files.Single();
            }

            var tasks = new List<Task>();
            while (queue.HasFilesToMerge)
            {
                var filesToMerge = queue.GetNextBatchOfFilesToMerge();
                
                var t = Task.Run(async () =>
                                {
                                    var thisMergeRunCounter = Interlocked.Increment(ref mergeRunCounter);
                                    var mergedFile = await merger.MergeFiles(filesToMerge, thisMergeRunCounter, cancellationToken);
                                    queue.AddMergedFile(mergedFile);

                                },
                                cancellationToken);
                tasks.Add(t);
            }
            Task.WaitAll([.. tasks], cancellationToken);
        }

        throw new Exception("Should never happen!");
    }
}
