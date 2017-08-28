using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Models
{
    public class IMPerson : IIdentifiableItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("twitterId")]
        public string TwitterId { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }

        [JsonProperty("facePersonId")]
        public string FacePersonId { get; set; }

        [JsonProperty("facePersonGroupId")]
        public string FacePersonGroupId { get; set; }

        [JsonProperty("speakerIdentificationProfileId")]
        public string SpeakerIdentificationProfileId { get; set; }

        [JsonProperty("audioEnrollmentBlobUri")]
        public string AudioEnrollmentBlobUri { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
