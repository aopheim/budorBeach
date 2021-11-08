using System;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Shared.Azure
{
    public static class AzureStorageHelper
    {
        public static BlobContainerClient GetBlobContainerClient(IConfiguration config, string blobContainerName)
        {
            var blobServiceClient = new BlobServiceClient(config["AzureStorageConnectionString"]);
            return blobServiceClient.GetBlobContainerClient(blobContainerName);
        }

        public static string GetUrlForBlob(this BlobItem blob, BlobContainerClient containerClient)
        {
            return new Uri(containerClient.Uri, $"{containerClient.Name}/{blob.Name}")
                .ToString();
        }

        public static async Task SetBlobPropertiesAsync(BlobClient blob)
        {
            try
            {
                // Get the existing properties
                BlobProperties properties = await blob.GetPropertiesAsync();

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
                await blob.SetHttpHeadersAsync(headers);
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}