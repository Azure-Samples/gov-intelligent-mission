using IntelligentMission.Web.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public class VideoApiClient
    {
        private const string baseUrl = "https://westus.api.cognitive.microsoft.com/video/v1.0";
        private IMConfig config;

        public VideoApiClient(IMConfig config)
        {
            this.config = config;
        }

        public async Task<dynamic> DetectMotion(string videoUri)
        {
            var initResponse = await this.InitiateDetectMotion(videoUri);
            if (initResponse.Headers.TryGetValues("Operation-Location", out IEnumerable<string> values))
            {
                string operationUri = values.FirstOrDefault();
                string operationStatus = null;
                dynamic response = null;

                do
                {
                    Thread.Sleep(12000);
                    response = await this.CheckVideoOperationResults(operationUri);
                    operationStatus = response.status;

                    if (response.error != null)
                    {
                        return response;
                    }

                } while (operationStatus != "Failed" && operationStatus != "Succeeded");
                return response;
            }

            throw new InvalidOperationException("Did not get proper response headers back from Video API.");
        }

        #region Private Methods

        private async Task<HttpResponseMessage> InitiateDetectMotion(string videoUri)
        {
            using (var httpClient = CreateHttpClient())
            {
                var request = new
                {
                    url = videoUri
                };
                var response = await httpClient.PostAsync($"{baseUrl}/detectmotion", request.ToStringContent());
                return response;
            }
        }

        private async Task<dynamic> CheckVideoOperationResults(string operationUri)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync(operationUri);
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JObject.Parse(json);
                return result;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(Constants.OcpSubscriptionKey, this.config.Keys.VideoApiKey);
            return httpClient;
        }

        #endregion
    }
}
