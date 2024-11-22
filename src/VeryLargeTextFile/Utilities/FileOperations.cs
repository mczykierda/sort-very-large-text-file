
using Microsoft.Extensions.Logging;

namespace VeryLargeTextFile.Utilities;

class FileOperations(
    ILogger<FileOperations> logger
    ) 
    : IFileOperations
{
    public void Move(FileInfo source, FileInfo target, bool overwriteOutputFile)
    {
        File.Move(source.FullName, target.FullName, overwriteOutputFile);
        logger.LogDebug("File {source} moved to {target}", source.FullName, target.FullName);
    }

    public void Delete(FileInfo fileInfo)
    {
        File.Delete(fileInfo.FullName);
        logger.LogDebug("File {file} deleted", fileInfo.Name);
    }
}
