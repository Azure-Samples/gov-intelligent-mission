using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IntelligentMission.Web.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public class StorageClient : IStorageClient
    {
        private BlobServiceClient blobServiceClient;
        private IMConfig config;

        public StorageClient(BlobServiceClient blobServiceClient, IMConfig config)
        {
            this.blobServiceClient = blobServiceClient;
            this.config = config;
        }

        public async Task<string> AddNewCatalogBlob(IFormFile blob)
        {
            var blobName = $"{Guid.NewGuid()}-{blob.FileName}";
            return await AddBlob(Containers.CatalogFiles, blobName, blob);
        }

        public async Task<string> AddNewAudioEnrollmentBlob(IFormFile blob)
        {
            var blobName = $"{Guid.NewGuid()}-{blob.FileName}";
            return await AddBlob(Containers.AudioEnrollments, blobName, blob);
        }


        private async Task<string> AddBlob(string containerName, string blobName, IFormFile blob)
        {
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            var blockBlob = container.GetBlobClient(blobName);
            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = blob.ContentType;

            using (var stream = blob.OpenReadStream())
            {
                await blockBlob.UploadAsync(stream, blobHttpHeaders);
            }
            return blockBlob.Uri.ToString();
        }

        public async Task<string> AddNewBlob(string personGroupId, string personId, IFormFile blob)
        {
            var container = blobServiceClient.GetBlobContainerClient(Containers.PersonFaces);
            await container.CreateIfNotExistsAsync();
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            var blobName = GetBlobName(personGroupId, personId, $"{Guid.NewGuid()}-{blob.FileName.Replace(' ', '-')}");
            var blockBlob = container.GetBlobClient(blobName);
            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = blob.ContentType;

            using (var stream = blob.OpenReadStream())
            {
                await blockBlob.UploadAsync(stream, blobHttpHeaders);
            }
            return blockBlob.Uri.ToString();
        }

        public BlobClient GetCatalogFileBlobByUri(string blobUri) => GetBlobByUri(blobUri, Containers.CatalogFiles);


        public BlobClient GetAudioEnrollmentBlobByUri(string blobUri) => GetBlobByUri(blobUri, Containers.AudioEnrollments);


        private BlobClient GetBlobByUri(string blobUri, string containerName)
        {
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            var blobName = blobUri.Substring(blobUri.LastIndexOf("/") + 1);
            var blob = container.GetBlobClient(blobName);
            return blob;
        }

        public async Task DeletePersonFaceBlob(string fullBlobUri)
        {
            await this.DeleteBlob(fullBlobUri, Containers.PersonFaces);
        }

        public async Task DeleteCatalogFileBlob(string fullBlobUri)
        {
            await this.DeleteBlob(fullBlobUri, Containers.CatalogFiles);
        }

        private async Task DeleteBlob(string fullBlobUri, string containerName)
        {
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            var blobName = ExtractBlobNameFromUri(fullBlobUri, containerName);
            var blockBlob = container.GetBlobClient(blobName);
            await blockBlob.DeleteAsync();
        }

        public async Task DeleteBlobs(string personGroupId, string personId)
        {
            var container = blobServiceClient.GetBlobContainerClient(Containers.PersonFaces);
            var prefix = $"groups/{personGroupId}/persons/{personId}";
            var blobs = container.GetBlobs(prefix: prefix);

            foreach (var blobItem in blobs)
            {
                var blob = container.GetBlobClient(blobItem.Name);
                await blob.DeleteAsync();
            }
        }

        #region Private Methods

        private static string GetBlobName(string personGroupId, string personId, string fileName) =>
            $"groups/{personGroupId}/persons/{personId}/{fileName}";

        private string ExtractBlobNameFromUri(string fullUri, string containerName)
        {
            string urlPrefix = $"https://{this.config.StorageConfig.AccountName}.blob.{this.config.StorageConfig.EndpointSuffix}/{containerName}/";
            return fullUri.Substring(urlPrefix.Length);
        }
        
        private static class Containers
        {
            public const string PersonFaces = "person-faces";
            public const string CatalogFiles = "catalog-files";
            public const string AudioEnrollments = "audio-enrollments";
        }

        #endregion
    }
}
