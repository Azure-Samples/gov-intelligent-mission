using IntelligentMission.Web.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public class VisionApiClient
    {
        private readonly string baseUrl;
        private IMConfig config;

        public VisionApiClient(IMConfig config)
        {
            this.config = config;
            this.baseUrl = this.config.CSEndpoints.ComputerVision;
        }

        public async Task<dynamic> AnalyzeImage(string imageUri)
        {
            using (var httpClient = CreateHttpClient())
            {
                var request = new
                {
                    url = imageUri
                };
                var response = await httpClient.PostAsync($"{baseUrl}/analyze?visualFeatures=Description,Categories,Faces,Tags,Color", request.ToStringContent());
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JObject.Parse(json);
                return result;
            }
        }

        public async Task<dynamic> OcrImage(string imageUri)
        {
            using (var httpClient = CreateHttpClient())
            {
                var request = new
                {
                    url = imageUri
                };
                var response = await httpClient.PostAsync($"{baseUrl}/ocr?language=en", request.ToStringContent());
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JObject.Parse(json);
                return result;
            }
        }


        #region Private Methods

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(Constants.OcpSubscriptionKey, this.config.Keys.ComputerVisionApiKey);
            return httpClient;
        }

        #endregion
    }
}
