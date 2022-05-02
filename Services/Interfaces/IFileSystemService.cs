using System.Collections.Generic;
using System.IO;

namespace Services.Interfaces
{
    public interface IFileSystemService
    {
        IEnumerable<string> GetFileNamesWithoutExtensionInFolder(string folderFilePath);
        void DeleteFile(string path);
        FileStreamWrapper GetFileStream(string filePath);
    }

    public class FileStreamWrapper
    {
        public FileStream FileStream { get; set; }
    }
}