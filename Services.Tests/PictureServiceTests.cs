using System;
using System.Linq;
using System.Threading.Tasks;
using BaseUnitTests;
using CameraService.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Services.Interfaces;
using Shared;
using Shared.PiCameraSettings;

namespace Services.Tests;

public class PictureServiceTests : UnitTestBase<PictureService>
{
    [Test]
    public async Task TakeImage_ShouldReceiveLocalJpgPath()
    {
        await TestSubject.TakeImageAndUploadAsync(new PiCameraSettings(), default);

        var calls = Get<ICameraService>().ReceivedCalls();

        var fullPath = calls.Single(c => c.GetMethodInfo().Name == nameof(ICameraService.TakeImage))
            .GetArguments().First();
        (fullPath as string).Should().EndWith(".jpg");
    }

    [Test]
    public async Task TakeImage_ShouldReceiveSettings()
    {
        var settings = new PiCameraSettings(200, 15);
        await TestSubject.TakeImageAndUploadAsync(settings, default);

        var calls = Get<ICameraService>().ReceivedCalls();

        var receivedSettings = calls.Single(c => c.GetMethodInfo().Name == nameof(ICameraService.TakeImage))
            .GetArguments()[1];
        (receivedSettings as PiCameraSettings).Should().BeEquivalentTo(settings);
    }

    [Test]
    public async Task TakenImage_ShouldFirstBeUploadedToImagesContainer()
    {
        await TestSubject.TakeImageAndUploadAsync(new PiCameraSettings(), default);

        var calls = Get<IAzureStorageService>().ReceivedCalls();

        var containerName = calls.First(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
            .GetArguments()[0];
        (containerName as string).Should().BeEquivalentTo(GlobalConstants.ImagesContainerName);
    }


    [Test]
    public async Task TakenImage_ShouldFirstBeUploadedToImagesContainer_FromCorrectPath()
    {
        await TestSubject.TakeImageAndUploadAsync(new PiCameraSettings(), default);

        var calls = Get<IAzureStorageService>().ReceivedCalls();

        var fullPath = calls.First(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
            .GetArguments()[1];
        (fullPath as string).Should().StartWith(GlobalConstants.ImagesFolder);
        (fullPath as string).Should().EndWith(".jpg");
    }

    [Test]
    public async Task TakenImage_ShouldFirstBeUploadedToImagesContainer_WithCorrectFilename()
    {
        await TestSubject.TakeImageAndUploadAsync(new PiCameraSettings(), default);

        var calls = Get<IAzureStorageService>().ReceivedCalls();

        var fileName = calls.First(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
            .GetArguments()[2];
        var currentYear = DateTime.UtcNow.Year.ToString();
        (fileName as string).Should().StartWith(currentYear);
        (fileName as string).Should().EndWith(".jpg");
    }

    [Test]
    public async Task ShouldHaveOnlyTwoUploadsToAzureStorage()
    {
        await TestSubject.TakeImageAndUploadAsync(new PiCameraSettings(), default);

        var calls = Get<IAzureStorageService>().ReceivedCalls();

        calls.Where(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath)).Should()
            .HaveCount(2);
    }

    [Test]
    public async Task CompressedImage_ShouldBeUploadedToThumbnailImagesContainer()
    {
        await TestSubject.TakeImageAndUploadAsync(new PiCameraSettings(), default);

        var calls = Get<IAzureStorageService>().ReceivedCalls();

        var containerName = calls.Last(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
            .GetArguments()[0];
        (containerName as string).Should().Be(GlobalConstants.ThumbnailImagesContainerName);
    }

    [Test]
    public async Task TakenImage_ShouldFirstBeUploadedToThumbnailImagesContainer_FromCorrectPath()
    {
        var inputPath = "my/input/path.jpg";
        Get<IPictureEditService>().CompressJpgToWebPFormat(Arg.Any<string>())
            .ReturnsForAnyArgs(inputPath.Replace(".jpg", ".webp"));

        await TestSubject.TakeImageAndUploadAsync(new PiCameraSettings(), default);

        var calls = Get<IAzureStorageService>().ReceivedCalls();

        var webpPath = calls.Last(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
            .GetArguments()[1] as string;
        webpPath.Should().EndWith(".webp");
    }

    [Test]
    public async Task TakenImage_ShouldFirstBeUploadedToThumbnailImagesContainer_WithCorrectFilename()
    {
        var inputPath = "my/input/path.jpg";
        Get<IPictureEditService>().CompressJpgToWebPFormat(Arg.Any<string>())
            .ReturnsForAnyArgs(inputPath.Replace(".jpg", ".webp"));

        await TestSubject.TakeImageAndUploadAsync(new PiCameraSettings(), default);

        var calls = Get<IAzureStorageService>().ReceivedCalls();
        var fileName = calls.Last(c => c.GetMethodInfo().Name == nameof(IAzureStorageService.UploadFileFromPath))
            .GetArguments()[2];
        var currentYear = DateTime.UtcNow.Year.ToString();
        (fileName as string).Should().StartWith(currentYear);
        (fileName as string).Should().EndWith(".webp");
    }
}