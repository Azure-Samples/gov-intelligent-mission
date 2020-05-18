using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace IntelligentMission.Web.Services
{
    public interface IStorageClient
    {
        Task<string> AddNewBlob(string personGroupId, string personId, IFormFile blob);
        Task<string> AddNewCatalogBlob(IFormFile blob);
        Task<string> AddNewAudioEnrollmentBlob(IFormFile blob);
        Task DeletePersonFaceBlob(string fullBlobUri);
        Task DeleteCatalogFileBlob(string fullBlobUri);
        Task DeleteBlobs(string personGroupId, string personId);
        BlobClient GetCatalogFileBlobByUri(string blobUri);
        BlobClient GetAudioEnrollmentBlobByUri(string blobUri);
    }
}