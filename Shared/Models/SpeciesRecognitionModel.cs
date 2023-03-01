using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class SpeciesRecognitionModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public double Confidence { get; set; }
        public string LatinName { get; set; }
        public string EnglishName { get; set; }
        public DateTime RecognizedAtUtc { get; set; }
        public Guid RecordingId { get; set; }
    }
}