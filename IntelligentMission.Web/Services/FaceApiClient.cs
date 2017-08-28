using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public class FaceApiClient
    {
        private FaceServiceClient faceServiceClient;

        public FaceApiClient(FaceServiceClient faceServiceClient)
        {
            this.faceServiceClient = faceServiceClient;
        }

        #region Person Groups

        public async Task<PersonGroup[]> GetPersonGroups()
        {
            var groups = await this.faceServiceClient.ListPersonGroupsAsync();
            return groups;
        }

        public async Task AddPersonGroup(PersonGroup group)
        {
            if (string.IsNullOrEmpty(group.PersonGroupId))
            {
                group.PersonGroupId = Guid.NewGuid().ToString();
            }

            await faceServiceClient.CreatePersonGroupAsync(group.PersonGroupId, group.Name);
        }

        public async Task DeletePersonGroup(string personGroupId)
        {
            await faceServiceClient.DeletePersonGroupAsync(personGroupId);
        }

        public async Task TrainPersonGroup(string personGroupId)
        {
            await faceServiceClient.TrainPersonGroupAsync(personGroupId);
        }

        public async Task<TrainingStatus> GetTrainingStatus(string personGroupId)
        {
            var trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);
            return trainingStatus;
        }

        #endregion

        #region Persons

        public async Task<CreatePersonResult> AddPerson(string personGroupId, string personName)
        {
            var person = await faceServiceClient.CreatePersonAsync(personGroupId, personName);
            return person;
        }

        public async Task<Person[]> GetPersonGroupPersonList(string personGroupId)
        {
            var personList = await faceServiceClient.GetPersonsAsync(personGroupId);
            return personList;
        }

        public async Task DeletePerson(string personGroupId, string personId)
        {
            await faceServiceClient.DeletePersonAsync(personGroupId, personId.ToGuid());
        }

        #endregion

        #region Faces

        public async Task<List<PersonFace>> GetPersonFaces(string personGroupId, string personId)
        {
            var pId = new Guid(personId);
            var person = await faceServiceClient.GetPersonAsync(personGroupId, pId);

            var personFaceList = new List<PersonFace>();
            foreach (var faceId in person.PersistedFaceIds)
            {
                var face = await faceServiceClient.GetPersonFaceAsync(personGroupId, pId, faceId);
                personFaceList.Add(face);
            }
            return personFaceList;
        }

        public async Task<Person> GetPerson(string personGroupdId, string personId)
        {
            var person = await this.faceServiceClient.GetPersonAsync(personGroupdId, personId.ToGuid());
            return person;
        }

        public async Task<PersonFace> GetPersonFace(string personGroupId, string personId, string faceId)
        {
            var face = await faceServiceClient.GetPersonFaceAsync(personGroupId, personId.ToGuid(), faceId.ToGuid());
            return face;
        }

        public async Task<AddPersistedFaceResult> AddPersonFace(string personGroupId, string personId, string faceUri)
        {
            var pId = new Guid(personId);
            var personFaceResult = await faceServiceClient.AddPersonFaceAsync(personGroupId, pId, faceUri, faceUri);
            return personFaceResult;
        }

        public async Task DeletePersonFace(string personGroupId, string personId, string faceId)
        {
            await faceServiceClient.DeletePersonFaceAsync(personGroupId, personId.ToGuid(), faceId.ToGuid());
        }

        #endregion

        #region Analysis Methods

        public async Task<Face[]> Detect(string imgUrl)
        {
            var faces = await this.faceServiceClient.DetectAsync(
                imageUrl: imgUrl,
                returnFaceAttributes: new[] {
                    FaceAttributeType.Gender,
                    FaceAttributeType.Smile,
                    FaceAttributeType.FacialHair,
                    FaceAttributeType.Glasses,
                    FaceAttributeType.Age,
                    FaceAttributeType.Emotion });
            return faces;
        }

        public async Task<IdentifyResult[]> Identify(string personGroupId, IEnumerable<Guid> faceIds)
        {
            var ids = faceIds.ToArray();
            return await this.faceServiceClient.IdentifyAsync(personGroupId, ids);
        }
    }
    #endregion
}
