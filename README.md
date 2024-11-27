# Very Large Text File

## TL;DR

*Note - Windows commands by default*

```
cd src
dotnet publish -c Release
cd VeryLargeTextFile\bin\Release\net8.0\publish\
```

Generate file of size 10GB
```
vltf generate -f 10gb.txt
```

Sort the file to get `10gb.txt.sorted` next to original file
```
vltf sort -f 10gb.txt -oof
```

Feel free to display help:
```
vltf -h
```

## Idea

The idea follows idea of family of merge sort algorithms.
Steps:
- split into number of smaller files (by default around 100MB each) to temp folder. Before saving - sort each of them (`Array.Sort` used).
- merge all splitted and sorted files into the final file. As the number of splitted sorted files can be large, this step actually uses intermediary files (i.e. take 16 files and merge into larger, then use that larger one as an input to merging with other files)

## Remarks

Initially I had a code which followed 3 steps sequentially: split, then sort, finally merge, but it was quite slow. 
I reworked the code to get to the stage where there is a bit of parallel processing:
- once we have buffer of bytes from input file we can delegate it to a task to get it sorted and saved as a sorted splitted file
- when we process the queueu of files to merge - once we get the the files to merge they can also be processed in a separate task.
