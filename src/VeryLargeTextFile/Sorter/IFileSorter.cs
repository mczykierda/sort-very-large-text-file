namespace VeryLargeTextFile.Sorter;

public interface IFileSorter
{
    Task SortFile(FileInfo inputFileInfo, FileInfo outputFileInfo, FileSortingConfig config, CancellationToken cancellationToken);
}
