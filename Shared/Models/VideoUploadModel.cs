using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class VideoUploadModel
{
    [Key] public string FileName { get; set; }
    public DateTime RecordedAtUtc { get; set; }
    public string VideoUrl { get; set; }
}
