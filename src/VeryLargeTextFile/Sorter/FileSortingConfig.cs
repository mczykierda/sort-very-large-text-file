using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Sorter.Merging;

namespace VeryLargeTextFile.Sorter;

public record FileSortingConfig(
    InputFileSplitterConfig Splitting, 
    MergeConfig Merging, 
    bool OverwriteOutputFile
    );