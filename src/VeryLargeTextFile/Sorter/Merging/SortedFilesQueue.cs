using System.Collections.Concurrent;

namespace VeryLargeTextFile.Sorter.Merging;

public class SortedFilesQueue(
    IReadOnlyCollection<FileInfo> initialSortedFiles, 
    MergeConfig config
    )
{
    readonly ConcurrentQueue<FileInfo> _files = new(initialSortedFiles);

    public IReadOnlyCollection<FileInfo> GetNextBatchOfFilesToMerge()
    {
        if (_files.Count > config.FileCountPerRun * 2)
        {
            return Dequeue(config.FileCountPerRun);
        }
        else if(_files.Count > config.FileCountPerRun) //last 2 batches more or less the same size
        {
            return Dequeue(_files.Count / 2);
        }
        else //final batch
        {
            return Dequeue(config.FileCountPerRun, true);
        }
    }

    List<FileInfo> Dequeue(int number, bool takeAll = false)
    {
        var result = new List<FileInfo>();
        while (!_files.IsEmpty)
        {
            _files.TryDequeue(out var file);
            result.Add(file!);
            if (result.Count == number && !takeAll)
            {
                break;
            }
        }
        return result;
    }

    public bool HasFilesToMerge => !_files.IsEmpty;
    public int Count => _files.Count;

    public void AddMergedFile(FileInfo file)
    {
        _files.Enqueue(file); 
    }
}