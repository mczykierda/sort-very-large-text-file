namespace VeryLargeTextFile.Sorter.FileSplitting;

public record SplittingResult(IReadOnlyCollection<SplittedFile> Files)
{
    public int MaxRecordCount { get; } = Files.Max(x => x.RecordCount);
}
