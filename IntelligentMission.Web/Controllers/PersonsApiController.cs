using IntelligentMission.Web.Models;
using IntelligentMission.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Controllers
{
    [Route("api/persons")]
    public class PersonsApiController : Controller
    {
        private PersonManager personManager;

        public PersonsApiController(PersonManager personManager)
        {
            this.personManager = personManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersons(string personGroupId, string speakerIdentificationProfileId)
        {
            if (!string.IsNullOrEmpty(personGroupId))
            {
                var list = await this.personManager.GetPersonsByGroup(personGroupId);
                return this.Ok(list);
            }
            else if (!string.IsNullOrEmpty(speakerIdentificationProfileId))
            {
                var person = this.personManager.GetPersonBySpeakerProfile(speakerIdentificationProfileId);
                return this.Ok(person);
            }
            else
            {
                throw new InvalidOperationException("Must specify either 'personGroupId' or 'speakerIdentificationProfileId' query string parameters.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(string id)
        {
            var person = await this.personManager.GetPerson(id);
            return this.Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody]IMPerson person)
        {
            await this.personManager.SaveNewPerson(person);
            return this.Ok(person);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson([FromBody]IMPerson person)
        {
            await this.personManager.UpdatePerson(person);
            return this.Ok(person);
        }
        
    }
}
