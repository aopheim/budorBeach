using System.Collections.Generic;
using System.IO;
using System.Linq;
using Services.Interfaces;

namespace Services
{
    public class FileSystemService : IFileSystemService
    {
        public IEnumerable<string> GetFileNamesWithoutExtensionInFolder(string folderFilePath)
        {
            var fullFileNames = Directory.GetFiles(folderFilePath);
            var recordingFileNames = fullFileNames.Select(Path.GetFileNameWithoutExtension).ToList();

            return recordingFileNames.Where(r => r != null);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public FileStreamWrapper GetFileStream(string filePath)
        {
            return new FileStreamWrapper
            {
                FileStream = new FileStream(filePath, FileMode.Open,
                    FileAccess.Read)
            };
        }
    }
}