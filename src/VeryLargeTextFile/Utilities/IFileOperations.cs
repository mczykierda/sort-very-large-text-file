
namespace VeryLargeTextFile.Utilities;

public interface IFileOperations
{
    void Move(FileInfo mergedFileInfo, FileInfo outputFileInfo, bool overwriteOutputFile);
    void Delete(FileInfo fileInfo);
}
