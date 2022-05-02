using System.Collections.Generic;

namespace Dtos
{
    public class BirdNetOutputDto
    {
        public string Message { get; set; }

        public List<ClassificationResultDto> Results { get; set; }
    }
}