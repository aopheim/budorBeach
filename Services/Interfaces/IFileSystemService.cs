using System;
using System.Collections.Generic;
using System.IO;

namespace Services.Interfaces
{
    public interface IFileSystemService
    {
        IEnumerable<string> GetFileNamesWithoutExtensionInFolder(string folderFilePath);
        void DeleteFile(string path);
        FileStream GetFileStream(string fullPath);
        void DeleteDirectory(string path, bool recursive);
        DateTime GetFileCreationTimeUtc(string fullPath);
    }
}