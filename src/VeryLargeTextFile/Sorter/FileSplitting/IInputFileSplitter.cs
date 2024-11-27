using System.Collections.ObjectModel;

namespace VeryLargeTextFile.Sorter.FileSplitting;

public interface IInputFileSplitter
{
    Task<IReadOnlyCollection<FileInfo>> SplitInputFileIntoSmallerFilesAndSortThem(FileInfo inputFileInfo, InputFileSplitterConfig config, CancellationToken cancellationToken);
}
