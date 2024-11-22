using VeryLargeTextFile.Utilities;

namespace VeryLargeTextFile.Sorter.Merging.SingleRun;

class SingleRunMerger(IInputFileStreamFactory inputFileStreamFactory,
    ITempFolderOperations tempFolder,
    IFileOperations fileOperations,
    IOutputFileStreamFactory outputFileStreamFactory,
    IComparer<string> comparer
    ) : ISingleRunMerger
{
    public async Task<FileInfo> MergeFiles(IEnumerable<FileInfo> sortedFiles, int mergeRunCounter, CancellationToken cancellationToken)
    {
        using var filesList = await CreateFilesList(sortedFiles);
        var mergedFileInfo = tempFolder.GetFileInfoForMergedFile(mergeRunCounter);
        using var outputWriter = new StreamWriter(outputFileStreamFactory.CreateOutputStream(mergedFileInfo));

        while (filesList.HasAnyFilesToProcess)
        {
            filesList.Sort(); //for single file - does nothing
            await outputWriter.WriteLineAsync(filesList.Head.Value);
            if (filesList.Head.IsFullyProcessed)
            {
                filesList.RemoveHeadFile();
            }
            else
            {
                await filesList.Head.MoveNext();
            }
        }

        return mergedFileInfo;
    }

    async Task<FilesList> CreateFilesList(IEnumerable<FileInfo> sortedFiles)
    {
        var result = new FilesList(sortedFiles.Count(), comparer);
        await result.AddFiles(sortedFiles.Select(x => new File(x, inputFileStreamFactory, fileOperations)));
        return result;
    }
}