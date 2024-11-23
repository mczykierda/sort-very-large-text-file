using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Sorter.Merging;
using VeryLargeTextFile.Sorter.SplittedFilesSorting;

namespace VeryLargeTextFile.Sorter;

public record FileSortingConfig(
    InputFileSplitterConfig Splitting, 
    SortConfig Sorting,
    MergeConfig Merging, 
    bool OverwriteOutputFile
    );