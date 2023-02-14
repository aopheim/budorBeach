using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Services;
using Services.Interfaces;
using Shared.Interfaces;
using Shared.Models;

namespace rpiDaemon.Test.Services
{
    public class AudioUploaderServiceTests : UnitTestBase<AudioUploaderService>
    {
        private readonly int MaxFilesToUpload = 10;

        [Test]
        public async Task IfRecordingIdsInFolder_NotExistInDb_DoNotUpload()
        {
            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() });
            Get<IRepositories>().SpeciesRecognitions.Find(srm => true)
                .ReturnsForAnyArgs(new List<SpeciesRecognitionModel>());

            await TestSubject.StartUpload(default);

            Get<IAzureStorageService>().ReceivedCalls().Should().BeEmpty();
        }

        [Test]
        public async Task IfRecordingIdsInFolder_ExistInDb_UploadRecordingIds()
        {
            var idsToUpload = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(idsToUpload.Select(id => id.ToString()));
            Get<IRepositories>().SpeciesRecognitions.Find(srm => true)
                .ReturnsForAnyArgs(new List<SpeciesRecognitionModel>
                {
                    new SpeciesRecognitionModel { RecordingId = idsToUpload[0] },
                    new SpeciesRecognitionModel { RecordingId = idsToUpload[1] }
                });

            await TestSubject.StartUpload(default);

            Get<IAzureStorageService>().ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath)).Should()
                .HaveCount(2);
        }

        [Test]
        public async Task OnlyUploadRecordingIds_WhichExistInDb()
        {
            var idToUpload = Guid.NewGuid();
            var idNotToUpload = Guid.NewGuid();
            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(new List<string> { idToUpload.ToString(), idNotToUpload.ToString() });
            Get<IRepositories>().SpeciesRecognitions.Find(srm => true)
                .ReturnsForAnyArgs(new List<SpeciesRecognitionModel>
                {
                    new SpeciesRecognitionModel { RecordingId = idToUpload },
                });

            await TestSubject.StartUpload(default);

            Get<IAzureStorageService>().ReceivedCalls()
                .Single(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
                .GetArguments()[2].Should().Be(idToUpload + ".wav");
        }

        [Test]
        public async Task DeleteRecording_AfterUpload()
        {
            var idsToUpload = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(idsToUpload.Select(id => id.ToString()));
            Get<IRepositories>().SpeciesRecognitions.Find(srm => true)
                .ReturnsForAnyArgs(new List<SpeciesRecognitionModel>
                {
                    new SpeciesRecognitionModel { RecordingId = idsToUpload[0] },
                    new SpeciesRecognitionModel { RecordingId = idsToUpload[1] }
                });

            await TestSubject.StartUpload(default);

            Get<IFileSystemService>().ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(IFileSystemService.DeleteFile)).Should().HaveCount(2);
        }

        [Test]
        public async Task ShouldNotUploadMoreThanLimit()
        {
            var idsToUpload = new List<Guid>();
            for (var i = 0; i < MaxFilesToUpload + 5; i++) idsToUpload.Add(Guid.NewGuid());

            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(idsToUpload.Select(id => id.ToString()));
            var recognitionsInDb = new List<SpeciesRecognitionModel>();
            idsToUpload.ForEach(id => recognitionsInDb.Add(new SpeciesRecognitionModel { RecordingId = id }));
            Get<IRepositories>().SpeciesRecognitions.Find(srm => true)
                .ReturnsForAnyArgs(recognitionsInDb);

            await TestSubject.StartUpload(default);

            Get<IAzureStorageService>().ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath)).Should()
                .HaveCount(MaxFilesToUpload);
        }
    }
}