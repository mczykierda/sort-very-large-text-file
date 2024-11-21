using System.Collections.ObjectModel;

namespace VeryLargeTextFile.Sorter.FileSplitting;

public interface IInputFileSplitter
{
    Task<SplittingResult> SplitInputFileIntoSmallerFilesAndSortThem(FileInfo inputFileInfo, InputFileSplitterConfig config, CancellationToken cancellationToken);
}
