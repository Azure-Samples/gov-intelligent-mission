using IntelligentMission.Web.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public class AudioManager
    {
        private ISpeakerIdApiClient speakerClient;
        private IStorageClient storageClient;
        private IMDbRepository repository;

        public AudioManager(ISpeakerIdApiClient speakerClient, IStorageClient storageClient, IMDbRepository repository)
        {
            this.speakerClient = speakerClient;
            this.storageClient = storageClient;
            this.repository = repository;
        }

        public async Task<List<CatalogFile>> GetAudioCatalogFiles()
        {
            var files = await this.repository.GetCatalogFiles(FileType.Audio);
            return files;
        }

        public async Task<CatalogFile> GetAudioCatalogFile(string audioId)
        {
            var file = await this.repository.GetCatalogFile(audioId, FileType.Audio);
            return file;
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
                FileType = FileType.Audio
            };

            await this.repository.CreateDoc(catalogFile);
            return catalogFile;
        }
        public async Task<dynamic> Enroll(string id, IFormFile uploadFile)
        {
            // 1. Add this file to blob storage (maybe we don't need this? send stream directly to Enrollment API?)
            var blobUri = await this.storageClient.AddNewAudioEnrollmentBlob(uploadFile);

            // 2. Store the blobUri in the person's document in the DB
            var person = await this.repository.GetPersonDoc(id);
            person.AudioEnrollmentBlobUri = blobUri;
            await this.repository.UpdatePersonDoc(person);

            // 3. Send this blob to the Enrollment API
            var blob = this.storageClient.GetAudioEnrollmentBlobByUri(blobUri);
            var enrollment = await this.speakerClient.CreateEnrollment(person.SpeakerIdentificationProfileId, blob);
            return enrollment;
        }


        public async Task<dynamic> Recognize(string audioId)
        {
            var file = await this.repository.GetCatalogFile(audioId, FileType.Audio);

            // get blob
            var blob = this.storageClient.GetCatalogFileBlobByUri(file.BlobUri);

            //send blob to recognize API
            var result = await this.speakerClient.Identify(blob);
            return result;
        }
    }
}
