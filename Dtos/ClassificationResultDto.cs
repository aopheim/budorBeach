namespace Dtos
{
    public class ClassificationResultDto
    {
        public double Confidence { get; set; }
        public string? LatinName { get; set; }
        public string? EnglishName { get; set; }
        public string? NorwegianName { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        
    }
}