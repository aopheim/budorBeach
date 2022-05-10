using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dtos;
using NSubstitute;
using NUnit.Framework;
using rpiDaemon.Jobs;
using Services.Interfaces;
using Shared.Interfaces;
using Shared.Models;

namespace rpiDaemon.Test.Jobs
{
    public class AnalyzeAudioRecordingsJobTests : UnitTestBase<AnalyzeAudioRecordingsJob>
    {
        private static readonly double MinConfidenceLevel = 0.5;
        private static readonly int MaxNumberOfFilesToAnalyze = 15;
        private static readonly string HumanLatinName = "Homo Sapiens";
        private static readonly string HumanEnglishName = "Human";

        private readonly List<string> _recordingIds = new()
        {
            "1fb49155-cd37-4525-a73c-5b4137fe7e28",
            "411ff434-4c08-434d-b01e-9707e703efc1",
            "fcdf0a93-976c-4129-bc5a-e45b20c08171",
            "e70dc45f-090b-4022-9742-20b80df49646"
        };

        [Test]
        public async Task NoRecordingsAboveMinConfidence_ShouldDeleteAudioRecording()
        {
            SetupMocking();
            Get<IBirdNetResultConverter>().ConvertJson(Arg.Any<string>()).ReturnsForAnyArgs(new BirdNetOutputDto
            {
                Message = "success",
                Results = new List<ClassificationResultDto>
                {
                    new()
                    {
                        Confidence = 0.2
                    },
                    new()
                    {
                        Confidence = 0.2
                    },
                    new()
                    {
                        Confidence = 0.1
                    }
                }
            });

            await TestSubject.Execute(default);

            Get<IFileSystemService>().Received(4).DeleteFile(Arg.Any<string>());
        }

        [Test]
        public async Task IfHumanSpeciesIsDetected_DoNotUploadRecording()
        {
            SetupMocking();
            Get<IBirdNetResultConverter>().ConvertJson(Arg.Any<string>()).ReturnsForAnyArgs(new BirdNetOutputDto
            {
                Message = "success",
                Results = new List<ClassificationResultDto>
                {
                    new()
                    {
                        LatinName = HumanLatinName,
                        Confidence = MinConfidenceLevel + 0.5,
                        EnglishName = HumanEnglishName
                    }
                }
            });

            await TestSubject.Execute(default);

            Get<IFileSystemService>().Received(4).DeleteFile(Arg.Any<string>());
            Get<IRepositories>().SpeciesRecognitions
                .AddRange(Arg.Is<List<SpeciesRecognitionModel>>(models => models.Count == 0));
        }

        [Test]
        public async Task IfHumanSpeciesIsDetected_EvenThoughBirdSpeciesIsAboveMinLevel()
        {
            SetupMocking();
            Get<IBirdNetResultConverter>().ConvertJson(Arg.Any<string>()).ReturnsForAnyArgs(new BirdNetOutputDto
            {
                Message = "success",
                Results = new List<ClassificationResultDto>
                {
                    new()
                    {
                        LatinName = HumanLatinName,
                        Confidence = 0.01,
                        EnglishName = HumanEnglishName
                    },
                    new()
                    {
                        LatinName = "Some Bird",
                        Confidence = 1.0,
                        EnglishName = "Some Bird"
                    }
                }
            });

            await TestSubject.Execute(default);

            Get<IFileSystemService>().Received(4).DeleteFile(Arg.Any<string>());
            Get<IRepositories>().SpeciesRecognitions
                .AddRange(Arg.Is<List<SpeciesRecognitionModel>>(models => models.Count == 0));
        }

        [Test]
        public async Task OnlySpeciesWithConfidenceAboveLimitShouldBeSaved()
        {
            SetupMocking();
            Get<IBirdNetResultConverter>().ConvertJson(Arg.Any<string>()).ReturnsForAnyArgs(new BirdNetOutputDto
            {
                Message = "success",
                Results = new List<ClassificationResultDto>
                {
                    new()
                    {
                        Confidence = MinConfidenceLevel + 0.01,
                        EnglishName = "ToSave"
                    },
                    new()
                    {
                        Confidence = MinConfidenceLevel - 0.02
                    },
                    new()
                    {
                        Confidence = MinConfidenceLevel - 0.2
                    }
                }
            });

            await TestSubject.Execute(default);

            Get<IRepositories>().SpeciesRecognitions.AddRange(Arg.Is<List<SpeciesRecognitionModel>>(models =>
                models.Select(m => m.EnglishName).Distinct().Single() == "ToSave"));
        }

        [Test]
        public async Task IfMoreFilesThanThresholdIsFoundForAnalysis_OnlyAnalyzeThresholdAmount()
        {
            SetupMocking();
            // Overwriting setup
            var fileIdsToReturn = new List<string>();
            for (var i = 0; i < MaxNumberOfFilesToAnalyze + 10; i++) fileIdsToReturn.Add(Guid.NewGuid().ToString());
            var fileMock = Get<IFileSystemService>();
            fileMock.GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(fileIdsToReturn);

            Get<IBirdNetResultConverter>().ConvertJson(Arg.Any<string>()).ReturnsForAnyArgs(new BirdNetOutputDto
            {
                Message = "success",
                Results = new List<ClassificationResultDto>
                {
                    new()
                    {
                        Confidence = MinConfidenceLevel + 0.01,
                        EnglishName = "ToSave"
                    }
                }
            });

            await TestSubject.Execute(default);

            Get<IRepositories>().SpeciesRecognitions
                .AddRange(Arg.Is<List<SpeciesRecognitionModel>>(models => models.Count <= MaxNumberOfFilesToAnalyze));
        }

        [Test]
        public async Task IfRecordingIdAlreadyExistsInDb_DoNotAnalyze()
        {
            SetupMocking();
            // Overwriting setup
            Get<IRepositories>().SpeciesRecognitions.Exists(Arg.Any<Guid>()).ReturnsForAnyArgs(true);

            Get<IBirdNetResultConverter>().ConvertJson(Arg.Any<string>()).ReturnsForAnyArgs(new BirdNetOutputDto
            {
                Message = "success",
                Results = new List<ClassificationResultDto>
                {
                    new()
                    {
                        Confidence = MinConfidenceLevel + 0.01,
                        EnglishName = "ToSave"
                    }
                }
            });

            await TestSubject.Execute(default);

            Get<IRepositories>().SpeciesRecognitions
                .AddRange(Arg.Is<List<SpeciesRecognitionModel>>(models => models.Count == 0));
        }

        private void SetupMocking()
        {
            var fileMock = Get<IFileSystemService>();
            fileMock.GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(_recordingIds);
            Get<IBirdNetServer>().PostAsync(Arg.Any<string>(), default).ReturnsForAnyArgs("");
        }
    }
}