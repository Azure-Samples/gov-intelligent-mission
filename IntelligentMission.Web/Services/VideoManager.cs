using IntelligentMission.Web.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public interface IVideoManager
    {
        Task<CatalogFile> UploadNewCatalogFile(CatalogFileModel file, IFormFile uploadFile);
        Task<List<CatalogFile>> GetVideoCatalogFiles();
        Task<CatalogFile> GetVideoCatalogFile(string id);
        Task DeleteCatalogFile(string id);
        Task<dynamic> DetectMotion(string id);
    }

    public class VideoManager : IVideoManager
    {
        private IStorageClient storageClient;
        private IMDbRepository repository;
        private VideoApiClient videoApi;

        public VideoManager(IStorageClient storageClient, IMDbRepository repository, VideoApiClient videoApi)
        {
            this.storageClient = storageClient;
            this.repository = repository;
            this.videoApi = videoApi;
        }

        public async Task<CatalogFile> UploadNewCatalogFile(CatalogFileModel file, IFormFile uploadFile)
        {
            var blobUrl = await this.storageClient.AddNewCatalogBlob(file.uploadFile);

            var catalogFile = new CatalogFile
            {
                Id = Guid.NewGuid().ToString(),
                Name = file.Name,
                Description = file.Description,
                BlobUri = blobUrl,
                FileType = FileType.Video
            };

            await this.repository.CreateDoc(catalogFile);
            return catalogFile;
        }

        public async Task<List<CatalogFile>> GetVideoCatalogFiles()
        {
            var files = await this.repository.GetCatalogFiles(FileType.Video);
            return files;
        }

        public async Task<CatalogFile> GetVideoCatalogFile(string id)
        {
            var file = await this.repository.GetCatalogFile(id, FileType.Video);
            return file;
        }

        public async Task DeleteCatalogFile(string id)
        {
            // Delete both the blob and the reference to it in the DB
            var file = await this.repository.GetCatalogFile(id, FileType.Video);
            await this.storageClient.DeleteCatalogFileBlob(file.BlobUri);
            await this.repository.DeleteCatalogFile(id, FileType.Video);
        }

        public async Task<dynamic> DetectMotion(string id)
        {
            var file = await this.repository.GetCatalogFile(id, FileType.Video);
            var response = await this.videoApi.DetectMotion(file.BlobUri);
            return response;
        }
    }
}
