using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Services;

public class FileSystemService : IFileSystemService
{
    private readonly ILogger<FileSystemService> _logger;

    public FileSystemService(ILogger<FileSystemService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<string> GetFileNamesWithoutExtensionInFolder(string folderFilePath)
    {
        var fullFileNames = Directory.GetFiles(folderFilePath);
        var recordingFileNames = fullFileNames?.Select(Path.GetFileNameWithoutExtension).ToList() ?? new List<string>();

        return recordingFileNames.Where(r => r != null);
    }

    public void DeleteFile(string path)
    {
        File.Delete(path);
    }
}