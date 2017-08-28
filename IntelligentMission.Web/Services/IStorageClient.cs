using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;

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
        CloudBlockBlob GetCatalogFileBlobByUri(string blobUri);
        CloudBlockBlob GetAudioEnrollmentBlobByUri(string blobUri);
    }
}