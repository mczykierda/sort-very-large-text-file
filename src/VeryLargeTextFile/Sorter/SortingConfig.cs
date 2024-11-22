using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Sorter.Merging;

namespace VeryLargeTextFile.Sorter;

public record SortingConfig(InputFileSplitterConfig Splitting, MergeConfig Merging, bool OverwriteOutputFile);