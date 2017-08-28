using IntelligentMission.Web.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public interface ISpeakerIdApiClient
    {
        Task<dynamic> Identify(CloudBlockBlob blob);
        Task<string> CreateProfile();
        Task<dynamic> CreateEnrollment(string identificationProfileId, CloudBlockBlob blob);

    }
    public class SpeakerIdApiClient : ISpeakerIdApiClient
    {
        private const string baseUrl = "https://westus.api.cognitive.microsoft.com/spid/v1.0";
        private IMConfig config;

        public SpeakerIdApiClient(IMConfig config)
        {
            this.config = config;
        }

        public async Task<string> CreateProfile()
        {
            using (var httpClient = CreateHttpClient())
            {
                var request = new { locale = "en-us" };
                var response = await httpClient.PostAsync($"{baseUrl}/identificationProfiles", request.ToStringContent());
                var json = await response.Content.ReadAsStringAsync();
                dynamic profile = JObject.Parse(json);
                return (string)profile.identificationProfileId;
            }
        }

        public async Task<dynamic> CreateEnrollment(string identificationProfileId, CloudBlockBlob blob)
        {
            using (var httpClient = CreateHttpClient())
            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);
                stream.Position = 0;


                var enrollmentUrl = $"{baseUrl}/identificationProfiles/{identificationProfileId}/enroll?shortAudio=true";
                var operationUrl = await OperationAsync(enrollmentUrl, stream);
                dynamic result = null;
                do
                {
                    Thread.Sleep(2000);
                    result = await GetOperationResult(operationUrl);

                } while (result.status == "notstated" || result.status == "running");

                return result;
            }
        }

        public async Task<dynamic> Identify(CloudBlockBlob blob)
        {
            // NOTE: API can only support at most 10 speakerIdentificationProfileIds for one identification request so we need to page them
            var speakerIdentificationProfileIds = await this.GetValidIdentificationProfileIds();
            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);
                var skipToken = 0;
                const int pageSize = 10;
                var idSegments = Enumerable.Empty<string>();
                var resultsList = new List<dynamic>();

                do
                {
                    idSegments = speakerIdentificationProfileIds.Skip(skipToken).Take(pageSize);
                    if (idSegments.Any())
                    {
                        dynamic results = await IdentifyBatch(idSegments.ToArray(), stream);
                        resultsList.Add(results);
                        // As soon as we find legitiamte results, bail.
                        var identifiedProfile = (string)results.processingResult.identifiedProfileId;
                        if (Guid.TryParse(identifiedProfile, out Guid identifiedId))
                        {
                            if (identifiedId != Guid.Empty)
                            {
                                return results;
                            }
                        }
                    }
                    skipToken += pageSize;

                } while (idSegments.Any());

                // If we've made it this far, we haven't found any legitimate results so just send back
                // the last result (rudimentary, but certainly fine for demo purposes).
                // If we did get multiple hits, we could send them all back. Or even have logic
                // to determine which results (in the case of multiple) to send back.
                return resultsList.Last();
            }
        }

        private async Task<string[]> GetValidIdentificationProfileIds()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"{baseUrl}/identificationProfiles");
                var json = await response.Content.ReadAsStringAsync();
                var profiles = JArray.Parse(json);
                var ids = profiles.Where(x => (string)x["enrollmentStatus"] == "Enrolled").Select(x => (string)x["identificationProfileId"]).ToArray();
                return ids;
            }
        }


        #region Private Methods

        private async Task<dynamic> IdentifyBatch(string[] ids, MemoryStream stream)
        {
            var idList = string.Join(",", ids);
            var queryString = $"identificationProfileIds={idList}&shortAudio=true";

            stream.Position = 0;
            var operationUrl = await OperationAsync($"{baseUrl}/identify?{queryString}", stream);
            dynamic result = null;
            do
            {
                Thread.Sleep(2000);
                result = await GetOperationResult(operationUrl);

            } while (result.status == "notstated" || result.status == "running");

            return result;
        }

        private async Task<object> GetOperationResult(string operationUrl)
        {
            var httpClient = CreateHttpClient();
            var response = await httpClient.GetAsync(operationUrl);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(json);
            return result;
        }

        private async Task<string> OperationAsync(string requestUri, Stream audioStream) 
        {
            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("u"));
            content.Add(new StreamContent(audioStream), "Data", "testFile_" + DateTime.Now.ToString("u"));
            var httpClient = CreateHttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(requestUri, content).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                IEnumerable<string> operationLocation = response.Headers.GetValues("Operation-Location");
                if (operationLocation.Count() == 1)
                {
                    string operationUrl = operationLocation.First();
                    return operationUrl;
                }
                else
                {
                    throw new InvalidOperationException("Incorrect server response");
                }
            }
            else
            {
                string resultStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new InvalidOperationException(resultStr);
            }
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(Constants.OcpSubscriptionKey, this.config.Keys.SpeakerRecognitionKey);
            return httpClient;
        }

        #endregion
    }
}
