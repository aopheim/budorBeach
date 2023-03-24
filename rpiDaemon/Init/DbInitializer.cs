using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace rpiDaemon.Init
{
    public class DbInitializer
    {
        public static void Initialize(BudorDbContext context, ILogger<Program> logger)
        {
            context.Database.Migrate();

            if (!context.SensorReadings.Any())
            {
                var readings = new List<SensorReadingModel>();

                for (var i = 0; i < 50; i++)
                {
                    var randomDouble = new Random().NextDouble();
                    readings.Add(new SensorReadingModel
                        { MeasuredAtUtc = DateTime.UtcNow, TemperatureInDegreesC = randomDouble });
                }

                foreach (var reading in readings) context.SensorReadings.Add(reading);

                context.SaveChanges();
            }

            if (!context.SpeciesRecognitions.Any())
            {
                var recognitions = new List<SpeciesRecognitionModel>();
                for (var i = 0; i < 50; i++)
                    recognitions.Add(new SpeciesRecognitionModel
                    {
                        Confidence = new Random().NextDouble(),
                        EnglishName = "SomeEnglishName",
                        LatinName = "SomeLatinName",
                        RecordingId = Guid.NewGuid(),
                        RecognizedAtUtc = DateTime.UtcNow
                    });

                foreach (var recognitionModel in recognitions) context.SpeciesRecognitions.Add(recognitionModel);
                context.SaveChanges();
            }

            logger.LogInformation("Db successfully initialized");
        }
    }
}