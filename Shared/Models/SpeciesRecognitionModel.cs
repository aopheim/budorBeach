using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

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
        [CanBeNull] public string EBirdTaxonomyId { get; set; }
        public DateTime RecognizedAtUtc { get; set; }
        public Guid RecordingId { get; set; }
    }
}