namespace VeryLargeTextFile.Sorter.Merging.SingleRun;

public class FilesList(int capacity, IComparer<string> comparer)
    : IDisposable
{
    readonly List<File> _list = new(capacity);

    public bool HasAnyFilesToProcess => _list.Count > 0;

    public void Sort()
        => _list.Sort((x, y) => comparer.Compare(x.Value, y.Value));

    public async Task AddFiles(IEnumerable<File> files)
    {
        _list.AddRange(files);
        //positiona at first row
        foreach (var file in _list)
        {
            await file.MoveNext();
        }
    }

    public File Head => _list[0];

    public void RemoveHeadFile()
    {
        Head.Dispose();
        _list.RemoveAt(0);
    }

    public void Dispose()
    {
        foreach (var file in _list)
        {
            file.Dispose();
        }
        _list.Clear();
    }
}