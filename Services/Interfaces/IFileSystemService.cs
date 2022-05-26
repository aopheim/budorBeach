using System.Collections.Generic;

namespace Services.Interfaces
{
    public interface IFileSystemService
    {
        IEnumerable<string> GetFileNamesWithoutExtensionInFolder(string folderFilePath);
        void DeleteFile(string path);
    }
}