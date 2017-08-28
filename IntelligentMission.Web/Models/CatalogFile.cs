using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Models
{
    public class CatalogFile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("blobUri")]
        public string BlobUri { get; set; }

        [JsonProperty("fileType")]
        public FileType FileType { get; set; }
    }

    public class CatalogFileModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile uploadFile { get; set; }

    }

    public enum FileType
    {
        Invalid,
        Image,
        Audio,
        Video
    }
}
