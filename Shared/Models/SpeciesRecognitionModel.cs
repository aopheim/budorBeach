using System;

namespace Shared.Models
{
    public class SpeciesRecognitionModel
    {
        public double Confidence { get; set; }
        public string LatinName { get; set; }
        public string EnglishName { get; set; }
        public DateTime RecognizedAtUtc { get; set; }
        public Guid RecordingId { get; set; }
    }
}