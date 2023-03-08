using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using SixLabors.ImageSharp;

namespace Services;

public class ImageConverter : IImageConverter
{
    private readonly ILogger<ImageConverter> _logger;

    public ImageConverter(ILogger<ImageConverter> logger)
    {
        _logger = logger;
    }

    public async Task<Stream> Jpg2WebP(Stream jpgStream, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting convert from jpg to WebP format");
        jpgStream.Position = 0;
        var image = await Image.LoadAsync(jpgStream, cancellationToken);
        if (image == null)
            throw new Exception("Failed to load jpg image");
        _logger.LogDebug("Image loaded");
        var outputStream = new MemoryStream();
        _logger.LogDebug("Created output stream");
        await image.SaveAsWebpAsync(outputStream, cancellationToken);
        _logger.LogDebug("Saved to stream");

        return outputStream;
    }
}