namespace VeryLargeTextFile.Sorter.FileSplitting;

public record SplittingResult(IEnumerable<SplittedFile> Files)
{
    public int MaxRecordCount => Files.Max(x => x.RecordCount);
}
