using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dtos;
using NSubstitute;
using NUnit.Framework;
using rpiDaemon.Jobs;
using Services.Interfaces;

namespace rpiDaemon.Test.Jobs
{
    public class AnalyzeAudioRecordingsJobTests : UnitTestBase<AnalyzeAudioRecordingsJob>
    {
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

        private void SetupMocking()
        {
            var fileMock = Get<IFileSystemService>();
            fileMock.GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(_recordingIds);
            fileMock.GetFileStream(Arg.Any<string>()).ReturnsForAnyArgs(new FileStreamWrapper());
            Get<IBirdNetServer>().PostAsync(Arg.Any<FileStream>(), default).ReturnsForAnyArgs("");
        }
    }
}