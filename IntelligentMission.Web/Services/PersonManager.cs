using IntelligentMission.Web.Models;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public class PersonManager
    {
        private IMDbRepository repository;
        private FaceApiClient faceApi;
        private ISpeakerIdApiClient speakerApi;

        public PersonManager(IMDbRepository repository, FaceApiClient faceApi, ISpeakerIdApiClient speakerApi)
        {
            this.repository = repository;
            this.faceApi = faceApi;
            this.speakerApi = speakerApi;

        }

        public async Task<IMPerson> GetPerson(string id)
        {
            var doc = await this.repository.GetPersonDoc(id);
            return doc;
        }

        public IMPerson GetPersonBySpeakerProfile(string speakerIdentificationProfileId)
        {
            return this.repository.GetPersonBySpeakerIdProfileId(speakerIdentificationProfileId);
        }

        public async Task<List<IMPerson>> GetPersonsByGroup(string personGroupId)
        {
            var persons = await this.repository.GetPersonsByGroup(personGroupId);
            return persons;
        }

        public async Task<IMPerson> SaveNewPerson(IMPerson person)
        {
            person.Id = Guid.NewGuid().ToString();
            
            // Create profile for Face
            var createPersonResult = await this.faceApi.AddPerson(person.FacePersonGroupId, person.FullName);
            person.FacePersonId = createPersonResult.PersonId.ToString();

            // Create profile for voice
            var speakerProfileId = await this.speakerApi.CreateProfile();
            person.SpeakerIdentificationProfileId = speakerProfileId;

            // Save to DB
            await this.repository.CreatePersonDoc(person);
            return person;
        }

        public async Task<IMPerson> UpdatePerson(IMPerson person)
        {
            await this.repository.UpdatePersonDoc(person);
            return person;
        }
    }
}
