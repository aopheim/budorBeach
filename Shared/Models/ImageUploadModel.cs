using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Shared.Models;

public class ImageUploadModel
{
    [Key] public string FileName { get; set; }
    public DateTime TakenAtUtc { get; set; }
    public string FullSizeImageUrl { get; set; }
    [CanBeNull] public string ThumbnailWebPImageUrl { get; set; }
    [CanBeNull] public string ThumbnailJpgImageUrl { get; set; }
}