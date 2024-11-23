# Very Large Text File

## TL;DR

*Note - Windows commands by default*

```
cd src
dotnet publish -c Release
cd VeryLargeTextFile\bin\Release\net8.0\publish\
```

Generate file of size 1GB with verbose output to console
```
vltf generate -f large_file.txt -v
```

Sort the file to get `large_file.txt.sorted` next to original file
```
vltf sort -f large_file.txt -oof -v
```

Feel free to display help:
```
vltf -h
```

## Idea

The idea follows idea of family of merge sort algorithms.
Steps:
- split into number of smaller files (by default around 100MB each) to temp folder
- sort each splitted file in memory (`Array.Sort` used) and save sorted files to temp folder. This step is parallized to speed up to whole process.
- merge all files into the final file. As the number of splitted sorted files can be large, this step actually uses intermediary files (i.e. take 10 files and merge into larger, then use that larger one as an input to merging with other files)
