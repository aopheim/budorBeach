using System;
using System.Collections.Generic;
using System.Linq;
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
            if (tmpDto?.Results == null || tmpDto.Results.Any() && tmpDto.Results.First().Count != 2)
                throw new ArgumentException("Received BirdNetOutput with more than two elements. Api has changed!");

            var result = new List<ClassificationResultDto>();
            var toReturn = new BirdNetOutputDto { Message = tmpDto.Message, Results = result };
            foreach (var detection in tmpDto.Results)
            {
                var stringJsonElement = (JsonElement)detection[0];
                var doubleJsonElement = (JsonElement)detection[1];

                var rawName = stringJsonElement.ValueKind == JsonValueKind.String ? stringJsonElement.GetString() : "";
                rawName ??= "";
                var confidence = doubleJsonElement.ValueKind == JsonValueKind.Number
                    ? doubleJsonElement.GetDouble()
                    : 0.0;

                var names = rawName.Split('_');
                if (names.Length != 2)
                    throw new Exception("Got names from BirdNet api in unknown format!");
                var latinName = names.First();
                var englishName = names.Last();
                result.Add(new ClassificationResultDto
                {
                    Confidence = confidence,
                    EnglishName = englishName,
                    LatinName = latinName
                });
            }

            return toReturn;
        }

        internal class BirdNetOutputTmpDto
        {
            [JsonPropertyName("msg")] public string Message { get; set; }

            [JsonPropertyName("results")] public IEnumerable<List<object>> Results { get; set; }
        }
    }
}