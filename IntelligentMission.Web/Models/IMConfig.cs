using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Models
{
    public class IMConfig
    {
        public CSEndpoints CSEndpoints { get; set; }
        public AppKeys Keys { get; set; }
        public StorageConfig StorageConfig { get; set; }
        public DocDbConfig DocDbConfig { get; set; }
    }

    public class AppKeys
    {
        public string TextTranslationKey { get; set; }
        public string TextAnalyticsKey { get; set; }
        public string SpeakerRecognitionKey { get; set; }
        public string FaceApiKey { get; set; }
        public string ComputerVisionApiKey { get; set; }
        public string VideoApiKey { get; set; }

    }

    public class StorageConfig
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string EndpointSuffix { get; set; }
    }

    public class DocDbConfig
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
    }

    public class CSEndpoints
    {
        public string ComputerVision { get; set; }
        public string TextTranslator { get; set; }
        public string TokenApi { get; set; }
        public string FaceApi { get; set; }
    }
}
