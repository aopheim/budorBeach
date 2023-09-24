using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
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
            Get<IRepositories>().SpeciesRecognitions.WhereAsync(srm => true, default)
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
            Get<IRepositories>().SpeciesRecognitions.WhereAsync(srm => true, default)
                .ReturnsForAnyArgs(new List<SpeciesRecognitionModel>
                {
                    new() { RecordingId = idsToUpload[0] },
                    new() { RecordingId = idsToUpload[1] }
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
            Get<IRepositories>().SpeciesRecognitions.WhereAsync(srm => true, default)
                .ReturnsForAnyArgs(new List<SpeciesRecognitionModel>
                {
                    new() { RecordingId = idToUpload }
                });

            await TestSubject.StartUpload(default);

            Get<IAzureStorageService>().ReceivedCalls()
                .Single(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
                .GetArguments()[2].Should().Be(idToUpload + ".wav");
        }
        
        [Test]
        public async Task WhenUploadingRecording_SetRecordingUploadedAtOnSpeciesRecognitionModel()
        {
            var idToUpload = Guid.NewGuid();
            var idNotToUpload = Guid.NewGuid();
            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(new List<string> { idToUpload.ToString(), idNotToUpload.ToString() });
            SpeciesRecognitionModel modelToUpload = new() { RecordingId = idToUpload };
            Get<IRepositories>().SpeciesRecognitions.WhereAsync(srm => true, default)
                .ReturnsForAnyArgs(new List<SpeciesRecognitionModel>
                {
                    modelToUpload
                });

            await TestSubject.StartUpload(default);

            await Get<IRepositories>().SpeciesRecognitions.Received(1)
                .UpdateAsync(Arg.Any<SpeciesRecognitionModel>(), default);
            var calls = Get<IRepositories>().SpeciesRecognitions.ReceivedCalls();
            var arg = calls.Single(c => c.GetMethodInfo().Name == nameof(ISpeciesRecognitionRepo.UpdateAsync))
                .GetArguments().First();
            var savedModel = (arg as SpeciesRecognitionModel);
            savedModel.RecordingUploadedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task DeleteRecording_AfterUpload()
        {
            var idsToUpload = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(idsToUpload.Select(id => id.ToString()));
            Get<IRepositories>().SpeciesRecognitions.WhereAsync(srm => true, default)
                .ReturnsForAnyArgs(new List<SpeciesRecognitionModel>
                {
                    new() { RecordingId = idsToUpload[0] },
                    new() { RecordingId = idsToUpload[1] }
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
            Get<IRepositories>().SpeciesRecognitions.WhereAsync(srm => true, default)
                .ReturnsForAnyArgs(recognitionsInDb);

            await TestSubject.StartUpload(default);

            Get<IAzureStorageService>().ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath)).Should()
                .HaveCount(MaxFilesToUpload);
        }

        [Test]
        public async Task ShouldNotUploadIfRecordingIsOfHiddenSpecies()
        {
            var recordingIdOfHiddenSpecies = Guid.NewGuid();
            var recordingIdToUpload = Guid.NewGuid();
            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(new List<string>
                    { recordingIdOfHiddenSpecies.ToString(), recordingIdToUpload.ToString() });
            Get<IRepositories>().SpeciesRecognitions.WhereAsync(srm => true, default)
                .ReturnsForAnyArgs(new List<SpeciesRecognitionModel>
                {
                    new() { RecordingId = recordingIdToUpload }
                });
            Get<IRepositories>().HiddenSpecies.GetAllAsync(default).ReturnsForAnyArgs(new List<HiddenSpeciesModel>
                { new() { TaxonomySpeciesId = "hiddenSpeciesId" } });

            await TestSubject.StartUpload(default);

            Get<IAzureStorageService>().ReceivedCalls()
                .Single(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
                .GetArguments()[2].Should().Be(recordingIdToUpload + ".wav");
        }

        [Test]
        public async Task IfMaxAmountHasBeenUploadedLast24Hours_NoRecordingShouldBeUploaded()
        {
            var recordingIdNotToUpload = Guid.NewGuid();
            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(new List<string>
                    { recordingIdNotToUpload.ToString() });
            Get<IRepositories>().SpeciesRecognitions.WhereAsync(srm => true, default)
                .ReturnsForAnyArgs(Fixture.CreateMany<SpeciesRecognitionModel>(AudioUploaderService.MaxUploadsIn24Hrs),
                    new List<SpeciesRecognitionModel>
                    {
                        new() { RecordingId = recordingIdNotToUpload }
                    });

            await TestSubject.StartUpload(default);

            Get<IAzureStorageService>().ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
                .Should().HaveCount(0);
        }

        [Test]
        public async Task IfMaxAmountIn24HoursWillBeReached_UploadNumberOfRecordingsToReachMax()
        {
            var idToUpload1 = Guid.NewGuid();
            var idToUpload2 = Guid.NewGuid();
            var idToUpload3 = Guid.NewGuid();
            var idNotToUpload = Guid.NewGuid();
            Get<IFileSystemService>().GetFileNamesWithoutExtensionInFolder(Arg.Any<string>())
                .ReturnsForAnyArgs(new List<string>
                {
                    idToUpload1.ToString(), idToUpload2.ToString(), idToUpload3.ToString(), idNotToUpload.ToString()
                });
            Get<IRepositories>().SpeciesRecognitions.WhereAsync(srm => true, default)
                .ReturnsForAnyArgs(
                    Fixture.CreateMany<SpeciesRecognitionModel>(AudioUploaderService.MaxUploadsIn24Hrs - 3),
                    new List<SpeciesRecognitionModel>
                    {
                        new() { RecordingId = idToUpload1 },
                        new() { RecordingId = idToUpload2 },
                        new() { RecordingId = idToUpload3 },
                        new() { RecordingId = idNotToUpload }
                    });

            await TestSubject.StartUpload(default);

            var calls = Get<IAzureStorageService>().ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath));
            calls.Select(call => call.GetArguments()[2]).Should().BeEquivalentTo(new[]
                { $"{idToUpload1}.wav", $"{idToUpload2}.wav", $"{idToUpload3}.wav" });
        }
    }
}