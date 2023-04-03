using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dtos;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Services;
using Services.Interfaces;
using Shared.Interfaces;
using Shared.Models;

namespace rpiDaemon.Test.Services
{
    public class AudioRecordingAnalyzerTests : UnitTestBase<AudioRecordingRecordingAnalyzer>
    {
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

        private static double MinConfidenceLevel => AudioRecordingRecordingAnalyzer.MinConfidenceLevel;

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
                    }, new()
                    {
                        Confidence = 0.2
                    }
                }
            });

            await TestSubject.RunAnalyzer(default);

            Get<IFileSystemService>().Received(4).DeleteFile(Arg.Any<string>());
            Get<IRepositories>().SpeciesRecognitions.Received(0).AddRange(Arg.Any<IEnumerable<SpeciesRecognitionModel>>());
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

            await TestSubject.RunAnalyzer(default);

            Get<IFileSystemService>().Received(4).DeleteFile(Arg.Any<string>());
            await Get<IRepositories>().SpeciesRecognitions
                .AddRangeAsync(Arg.Is<List<SpeciesRecognitionModel>>(models => models.Count == 0), default);
            var calls = Get<IRepositories>().ReceivedCalls();
            calls.Where(c => c.GetMethodInfo().Name == nameof(ISpeciesRecognitionRepo.AddRangeAsync)).Should()
                .BeEmpty();
        }

        [Test]
        public async Task IfHumanSpeciesIsDetected_DoNotAddToDb_EvenThoughBirdSpeciesIsAboveMinLevel()
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

            await TestSubject.RunAnalyzer(default);

            Get<IFileSystemService>().Received(4).DeleteFile(Arg.Any<string>());
            Get<IRepositories>().ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(ISpeciesRecognitionRepo.AddRangeAsync)).Should().BeEmpty();
        }

        [Test]
        public async Task OnlySpeciesWithConfidenceAboveLimitShouldBeSaved()
        {
            SetupMockingWithOneFile();
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

            await TestSubject.RunAnalyzer(default);

            Get<IRepositories>().SpeciesRecognitions.Received(1).AddRange(Arg.Any<IEnumerable<SpeciesRecognitionModel>>());
            var calls = Get<IRepositories>().SpeciesRecognitions.ReceivedCalls();
            var arg = calls.Single(c => c.GetMethodInfo().Name == nameof(ISpeciesRecognitionRepo.AddRangeAsync))
                .GetArguments().First();
            (arg as List<SpeciesRecognitionModel>).First().EnglishName.Should().Be("ToSave");
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

            await TestSubject.RunAnalyzer(default);

            var calls = Get<IRepositories>().SpeciesRecognitions.ReceivedCalls();
            var arg = calls.Single(c => c.GetMethodInfo().Name == nameof(ISpeciesRecognitionRepo.AddRangeAsync))
                .GetArguments().First();
            (arg as List<SpeciesRecognitionModel>).Count.Should().BeLessOrEqualTo(MaxNumberOfFilesToAnalyze);
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

            await TestSubject.RunAnalyzer(default);

            var calls = Get<IRepositories>().SpeciesRecognitions.ReceivedCalls();
            var arg = calls.Where(c => c.GetMethodInfo().Name == nameof(ISpeciesRecognitionRepo.AddRangeAsync)).Should()
                .HaveCount(0);
        }

        private void SetupMocking()
        {
            var fileMock = Get<IFileSystemService>();
            fileMock.GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(_recordingIds);
            Get<IBirdNetServer>().PostAsync(Arg.Any<string>(), default).ReturnsForAnyArgs("");
        }

        private void SetupMockingWithOneFile()
        {
            var fileMock = Get<IFileSystemService>();
            fileMock.GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(_recordingIds.Take(1));
            Get<IBirdNetServer>().PostAsync(Arg.Any<string>(), default).ReturnsForAnyArgs("");
        }
    }
}