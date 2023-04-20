using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class HiddenSpeciesModel
{
    [Key] public string TaxonomySpeciesId { get; set; }
}