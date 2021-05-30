using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace budorWeb.Helpers
{
    public static class AzureStorageHelper
    {
        public static BlobContainerClient GetBlobContainerClient(IConfiguration config, string blobContainerName)
        {
            var blobServiceClient = new BlobServiceClient(config.GetConnectionString("AzureStorageConnectionString"));
            return blobServiceClient.GetBlobContainerClient(blobContainerName);
        }

        public static string GetUrlForBlob(this BlobItem blob, BlobContainerClient containerClient)
        {
            return new Uri(containerClient.Uri, $"{containerClient.Name}/{blob.Name}")
                .ToString();
        }
    }
}