namespace VeryLargeTextFile.Generator;

public interface IFileGenerator
{
    Task GenerateFile(FileInfo fileInfo, long fileSize, int textSize, int textDuplicationFactor);
}


