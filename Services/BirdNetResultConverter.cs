using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dtos;
using Services.Interfaces;

namespace Services
{
    public class BirdNetResultConverter : IBirdNetResultConverter
    {
        public BirdNetOutputDto ConvertJson(string json)
        {
            var tmpDto = JsonSerializer.Deserialize<BirdNetOutputTmpDto>(json);
            if (tmpDto?.Message != "success")
                throw new ArgumentException("BirdNetServer did not return success");

            var result = new List<ClassificationResultDto>();
            var toReturn = new BirdNetOutputDto { Message = tmpDto.Message, Results = result };
            foreach (var detection in tmpDto.Results)
                result.Add(new ClassificationResultDto
                {
                    Confidence = detection.Confidence,
                    EnglishName = detection.CommonName,
                    LatinName = detection.LatinName,
                    StartTime = detection.StartTime,
                    EndTime = detection.EndTime,
                });

            return toReturn;
        }

        internal class BirdNetOutputTmpDto
        {
            [JsonPropertyName("msg")] public string Message { get; set; }

            [JsonPropertyName("results")] public IEnumerable<BirdNetClassificationDto> Results { get; set; }
        }

        internal class BirdNetClassificationDto
        {
            [JsonPropertyName("common_name")] public string CommonName { get; set; }

            [JsonPropertyName("start_time")] public double StartTime { get; set; }

            [JsonPropertyName("end_time")] public double EndTime { get; set; }

            [JsonPropertyName("scientific_name")] public string LatinName { get; set; }

            [JsonPropertyName("confidence")] public double Confidence { get; set; }
        }
    }
}