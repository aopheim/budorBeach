using System;
using System.Collections.Generic;
using System.IO;

namespace Services.Interfaces
{
    public interface IFileSystemService
    {
        IEnumerable<string> GetFileNamesWithoutExtensionInFolder(string folderFilePath);
        IEnumerable<string> GetFileNamesInFolder(string folderFilePath);
        IEnumerable<string> GetFolderNamesInFolder(string folderFilePath);
        void DeleteFile(string path);
        FileStream GetFileStream(string fullPath);
        void DeleteDirectory(string path, bool recursive);
        bool IsDirectoryEmpty(string path);
        DateTime GetFileCreationTimeUtc(string fullPath);
    }
}