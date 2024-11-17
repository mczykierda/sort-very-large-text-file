namespace VeryLargeTextFile.Generator;

public interface IFileGenerator
{
    Task GenerateFile(FileInfo fileInfo, long fileSize, long textSize, int textDuplicationFactor);
}
