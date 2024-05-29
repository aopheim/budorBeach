using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Services.Interfaces;
using Shared;

namespace Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private static readonly TimeSpan UploadTimeOut = TimeSpan.FromSeconds(30);
        private readonly BlobServiceClient _blobServiceClient;
        private readonly CancellationTokenSource _timeOutTokenSource = new CancellationTokenSource(UploadTimeOut);

        public AzureStorageService(IConfiguration config, IWebHostEnvironment environment)
        {
            _blobServiceClient =
                new BlobServiceClient(environment.IsDevelopment()
                        ? config[AzuriteStorageConnectionString]
                        : config[AzureStorageConnectionString],
                    new BlobClientOptions
                    {
                        Retry =
                        {
                            MaxRetries = 5, Delay = TimeSpan.FromSeconds(2), Mode = RetryMode.Exponential,
                            MaxDelay = TimeSpan.FromSeconds(30), NetworkTimeout = UploadTimeOut
                        }
                    });
        }

        private static string AzureStorageConnectionString => GlobalConstants.AzureStorageConnectionString;
        private static string AzuriteStorageConnectionString => GlobalConstants.AzuriteStorageConnectionString;


        public BlobContainerClient GetBlobContainerClient(string containerName)
        {
            return _blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<bool> UploadFileFromPath(string containerName, string filePath, string fileNameWithExtension,
            CancellationToken cancellationToken)
        {
            var blobClient = GetBlobClient(containerName, fileNameWithExtension);
            await using var uploadFileStream = File.OpenRead(filePath);
            var cTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _timeOutTokenSource.Token);
            await blobClient.UploadAsync(uploadFileStream, true, cTokenSource.Token);
            uploadFileStream.Close();

            return true;
        }

        public async Task UploadFileFromStream(Stream inputStream, string containerName, string fileNameWithExtension,
            CancellationToken cancellationToken)
        {
            var client = GetBlobClient(containerName, fileNameWithExtension);
            await client.UploadAsync(inputStream, true, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string containerName, string fileNameWithExtension,
            CancellationToken cancellationToken)
        {
            var blobClient = GetBlobClient(containerName, fileNameWithExtension);

            return await blobClient.ExistsAsync(cancellationToken);
        }

        public async Task SetJpgBlobPropertiesAsync(BlobClient blob, CancellationToken cancellationToken)
        {
            try
            {
                // Get the existing properties
                BlobProperties properties = await blob.GetPropertiesAsync(null, cancellationToken);

                var headers = new BlobHttpHeaders
                {
                    // Set the MIME ContentType every time the properties 
                    // are updated or the field will be cleared
                    ContentType = "image/jpeg",
                    ContentLanguage = "en-us",

                    // Populate remaining headers with 
                    // the pre-existing properties
                    CacheControl = properties.CacheControl,
                    ContentDisposition = properties.ContentDisposition,
                    ContentEncoding = properties.ContentEncoding,
                    ContentHash = properties.ContentHash
                };

                // Set the blob's properties.
                await blob.SetHttpHeadersAsync(headers, null, cancellationToken);
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        public string GetBlobUrl(string containerName, string fileNameWithExtension)
        {
            return GetBlobClient(containerName, fileNameWithExtension)?.Uri?.ToString();
        }

        public BlobClient GetBlobClient(string containerName, string fileNameWithExtension)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            containerClient.CreateIfNotExists();

            return containerClient.GetBlobClient(fileNameWithExtension);
        }
    }
}