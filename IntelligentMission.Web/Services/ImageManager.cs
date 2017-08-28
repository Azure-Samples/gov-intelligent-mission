using IntelligentMission.Web.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public interface IImageManager
    {
        Task DeleteCatalogFile(string id);
        Task<CatalogFile> UploadNewCatalogFile(CatalogFileModel file, IFormFile uploadFile);
    }

    public class ImageManager : IImageManager
    {
        private IMDbRepository repository;
        private IStorageClient storageClient;

        public ImageManager(IStorageClient storageClient, IMDbRepository repository)
        {
            this.storageClient = storageClient;
            this.repository = repository;
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
                FileType = FileType.Image
            };

            await this.repository.CreateDoc(catalogFile);
            return catalogFile;
        }

        public async Task DeleteCatalogFile(string id)
        {
            // Delete both the blob and the reference to it in the DB
            var file = await this.repository.GetCatalogFile(id, FileType.Image);
            await this.storageClient.DeleteCatalogFileBlob(file.BlobUri);
            await this.repository.DeleteCatalogFile(id, FileType.Image);
        }
    }
}
