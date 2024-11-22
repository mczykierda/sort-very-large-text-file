
namespace VeryLargeTextFile.Utilities;

class FileOperations : IFileOperations
{
    public void Move(FileInfo mergedFileInfo, FileInfo outputFileInfo, bool overwriteOutputFile)
        => File.Move(mergedFileInfo.FullName, outputFileInfo.FullName, overwriteOutputFile);

    public void Delete(FileInfo fileInfo)
        => File.Delete(fileInfo.FullName);
}
