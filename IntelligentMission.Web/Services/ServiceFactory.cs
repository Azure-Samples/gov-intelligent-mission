using IntelligentMission.Web.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.ProjectOxford.Face;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public class ServiceFactory
    {
        private IMConfig config;

        public ServiceFactory(IMConfig config)
        {
            this.config = config;
        }

        public CloudStorageAccount CreateCloudStorageAccount2() =>
            new CloudStorageAccount(
                new StorageCredentials(
                    accountName: this.config.StorageConfig.AccountName,
                    keyValue: this.config.StorageConfig.AccountKey),
                endpointSuffix: this.config.StorageConfig.EndpointSuffix,
                useHttps: true);

        public CloudBlobClient CreateCloudBlobClient() => this.CreateCloudStorageAccount2().CreateCloudBlobClient();

        //public FaceServiceClient CreateFaceServiceClient2() => new FaceServiceClient(this.config.Keys.FaceApiKey);
        public FaceServiceClient CreateFaceServiceClient2() => new FaceServiceClient(this.config.Keys.FaceApiKey, this.config.CSEndpoints.FaceApi);

        public DocumentClient CreateDocumentClient2() => new DocumentClient(new Uri(this.config.DocDbConfig.EndpointUri), this.config.DocDbConfig.PrimaryKey);

    }
}
