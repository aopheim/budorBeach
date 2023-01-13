using System.Threading.Tasks;
using Services.Interfaces;
using SixLabors.ImageSharp;

namespace Services;

public class PictureEditService : IPictureEditService
{
    private readonly IFileSystemService _fileSystemService;

    public PictureEditService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
    }

    public async Task<string> CompressJpgToWebPFormat(string fullInputJpgPath)
    {
        var outputPath = fullInputJpgPath.Replace(".jpg", ".webp");

        await using var input = _fileSystemService.GetFileStream(fullInputJpgPath);
        var image = await Image.LoadAsync(input);
        await image.SaveAsWebpAsync(outputPath);

        return outputPath;
    }
}