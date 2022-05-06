using System.Collections.Generic;
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

        private readonly List<string> _recordingIds = new List<string>
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
                    new ClassificationResultDto
                    {
                        Confidence = 0.2,
                    },
                    new ClassificationResultDto
                    {
                        Confidence = 0.2,
                    },
                    new ClassificationResultDto
                    {
                        Confidence = 0.1,
                    },
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
                    new ClassificationResultDto
                    {
                        LatinName = "Homo Sapiens",
                        Confidence = MinConfidenceLevel + 0.5,
                        EnglishName = "Human"
                    },
                    new ClassificationResultDto
                    {
                        Confidence = MinConfidenceLevel + 0.2,
                    },
                    new ClassificationResultDto
                    {
                        Confidence = MinConfidenceLevel + 0.2,
                    },
                }
            });

            await TestSubject.Execute(default);

            Get<IFileSystemService>().Received(4).DeleteFile(Arg.Any<string>());
            Get<IRepositories>().Received(0).SpeciesRecognitions.Add(Arg.Any<SpeciesRecognitionModel>());
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
                    new ClassificationResultDto
                    {
                        Confidence = MinConfidenceLevel + 0.01,
                        EnglishName = "ToSave"
                    },
                    new ClassificationResultDto
                    {
                        Confidence = MinConfidenceLevel - 0.2,
                    },
                    new ClassificationResultDto
                    {
                        Confidence = MinConfidenceLevel - 0.2,
                    },
                }
            });


            await TestSubject.Execute(default);

            Get<IRepositories>().Received(1).SpeciesRecognitions.Add(default);
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