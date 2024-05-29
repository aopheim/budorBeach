using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using SixLabors.ImageSharp;

namespace Services;

public class PictureEditService : IPictureEditService
{
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<PictureEditService> _logger;

    public PictureEditService(IFileSystemService fileSystemService, ILogger<PictureEditService> logger)
    {
        _fileSystemService = fileSystemService;
        _logger = logger;
    }

    public async Task<string> CompressJpgToWebPFormat(string fullInputJpgPath, CancellationToken cancellationToken)
    {
        var sw = new Stopwatch();
        sw.Start();
        var outputPath = fullInputJpgPath.Replace(".jpg", ".webp");
        await using var input = _fileSystemService.GetFileStream(fullInputJpgPath);
        var image = await Image.LoadAsync(input, cancellationToken);
        _logger.LogDebug($"Starting compressing to webp image after {sw.ElapsedMilliseconds} ms");
        await image.SaveAsWebpAsync(outputPath, cancellationToken);
        sw.Stop();
        _logger.LogDebug($"Compressed image {fullInputJpgPath} to webp in {sw.ElapsedMilliseconds} ms");

        return outputPath;
    }
}