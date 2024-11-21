namespace VeryLargeTextFile.Sorter.Comparer;

class RecordComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null && y != null) return -1;
        if (y == null && x != null) return 1;
        if (x == null || y == null) return 0;

        var xRecord = Record.Parse(x);
        var yRecord = Record.Parse(y);

        var result = xRecord.Text.CompareTo(yRecord.Text, StringComparison.Ordinal);
        if (result == 0)
        {
            result = xRecord.Number.CompareTo(yRecord.Number);
        }
        return result;
    }
}
