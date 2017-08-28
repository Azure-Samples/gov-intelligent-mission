using IntelligentMission.Web.Models;
using IntelligentMission.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/face")]
    public class FaceApiController : Controller
    {
        private FaceApiClient faceApi;
        private IStorageClient storageClient;
        private IMDbRepository repository;

        public FaceApiController(IStorageClient storageClient, FaceApiClient faceApi, IMDbRepository repository)
        {
            this.storageClient = storageClient;
            this.faceApi = faceApi;
            this.repository = repository;
        }

        #region Person Groups

        [HttpGet("person-groups")]
        public async Task<IActionResult> GetPersonGroups()
        {
            var groups = await this.faceApi.GetPersonGroups();
            return this.Ok(groups);
        }

        [HttpPost("person-groups")]
        public async Task<IActionResult> CreatePersonGroup([FromBody]PersonGroup group)
        {
            await this.faceApi.AddPersonGroup(group);
            return this.Ok(group);
        }

        [HttpDelete("person-groups/{personGroupId}")]
        public async Task<IActionResult> DeletePersonGroup(string personGroupId)
        {
            //TODO: delete corresponding resources in blob storage
            await this.faceApi.DeletePersonGroup(personGroupId);
            return this.NoContent();
        }

        #endregion

        #region Persons

        [HttpGet("person-groups/{personGroupId}/persons")]
        public async Task<IActionResult> GetGroupPersonList(string personGroupId)
        {
            var personList = await this.faceApi.GetPersonGroupPersonList(personGroupId);
            return this.Ok(personList);
        }

        [HttpPost("person-groups/{personGroupId}/persons")]
        public async Task<IActionResult> CreateGroupPerson(string personGroupId, [FromBody]IMPerson person)
        {
            var newPerson = await this.faceApi.AddPerson(personGroupId, person.FullName);
            //TODO: insert person metadata into DocDb
            return this.Ok(new Person { PersonId = newPerson.PersonId, Name = person.FullName });
        }

        [HttpDelete("person-groups/{personGroupId}/persons/{personId}")]
        public async Task<IActionResult> DeleteGroupPerson(string personGroupId, string personId)
        {
            //TODO: delete associated metadata in DocDb
            var blobs = this.storageClient.DeleteBlobs(personGroupId, personId);
            await this.faceApi.DeletePerson(personGroupId, personId);
            return this.NoContent();
        }

        [HttpPost("person-groups/{personGroupId}/train")]
        public async Task<IActionResult> TrainPersonGroup(string personGroupId)
        {
            await this.faceApi.TrainPersonGroup(personGroupId);
            return this.Ok();
        }

        [HttpGet("person-groups/{personGroupId}/train")]
        public async Task<IActionResult> GetTrainingGroupStatus(string personGroupId)
        {
            var trainingStatus = await this.faceApi.GetTrainingStatus(personGroupId);
            return this.Ok(trainingStatus);
        }

        #endregion

        #region Faces

        [HttpGet("person-groups/{personGroupId}/persons/{personId}/faces")]
        public async Task<IActionResult> GetPersonFaces(string personGroupId, string personId)
        {
            var faceList = await this.faceApi.GetPersonFaces(personGroupId, personId);
            return this.Ok(faceList);
        }

        [HttpPost("person-groups/{personGroupId}/persons/{personId}/faces")]
        public async Task<IActionResult> CreatePersonFace(string personGroupId, string personId, IFormFile uploadFile)
        {
            var blobUri = await this.storageClient.AddNewBlob(personGroupId, personId, uploadFile);
            var personFaceResult = await this.faceApi.AddPersonFace(personGroupId, personId, blobUri);

            var personFace = new PersonFace { PersistedFaceId = personFaceResult.PersistedFaceId, UserData = blobUri };
            return this.Ok(personFace);
        }

        [HttpDelete("person-groups/{personGroupId}/persons/{personId}/faces/{faceId}")]
        public async Task<IActionResult> DeletePersonFace(string personGroupId, string personId, string faceId)
        {
            var face = await this.faceApi.GetPersonFace(personGroupId, personId, faceId);
            await this.faceApi.DeletePersonFace(personGroupId, personId, faceId);
            await this.storageClient.DeletePersonFaceBlob(face.UserData);
            return this.NoContent();
        }

        #endregion

        #region Analysis

        [HttpPost("analysis/{id}/detect")]
        public async Task<IActionResult> Detect(string id)
        {
            var file = await this.repository.GetCatalogFile(id, FileType.Image);
            var result = await this.faceApi.Detect(file.BlobUri);
            return this.Ok(result);
        }

        #endregion
    }
}
