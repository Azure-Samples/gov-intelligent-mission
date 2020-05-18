using Azure.Storage;
using Azure.Storage.Blobs;
using IntelligentMission.Web.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.ProjectOxford.Face;
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

        public BlobServiceClient CreateBlobServiceClient2() =>
            new BlobServiceClient(new Uri(this.config.StorageConfig.EndpointSuffix),
                new StorageSharedKeyCredential( this.config.StorageConfig.AccountName,this.config.StorageConfig.AccountKey));

        public BlobServiceClient CreateBlobServiceClient() => this.CreateBlobServiceClient2();

        //public FaceServiceClient CreateFaceServiceClient2() => new FaceServiceClient(this.config.Keys.FaceApiKey);
        public FaceServiceClient CreateFaceServiceClient2() => new FaceServiceClient(this.config.Keys.FaceApiKey, this.config.CSEndpoints.FaceApi);

        public DocumentClient CreateDocumentClient2() => new DocumentClient(new Uri(this.config.DocDbConfig.EndpointUri), this.config.DocDbConfig.PrimaryKey);

    }
}
