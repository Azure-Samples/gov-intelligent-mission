using IntelligentMission.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IntelligentMission.Web.Services
{
    public interface ITranslationApiClient
    {
        Task<string> GetTranslation(string language, string textToTranslate);
    }

    public class TranslationApiClient : ITranslationApiClient
    {
        private IMemoryCache cache;
        private const string tokenKey = "access-cache-token-key";
        private IMConfig config;

        public TranslationApiClient(IMemoryCache cache, IMConfig config)
        {
            this.cache = cache;
            this.config = config;
        }

        //make  separate translation method for header
        public async Task<string> GetTranslation(string language, string textToTranslate)
        {
            var response = await this.GetTranslation(language, textToTranslate, this.config.Keys.TextTranslationKey);
            var result = await response.Content.ReadAsStringAsync();
            var translationResult = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(result);
            dynamic translation = translationResult.First();
            dynamic firstTranslation = (translation.translations as IEnumerable<dynamic>).First();
            return firstTranslation.text;
        }

        private async Task<HttpResponseMessage> GetTranslation(string language, string textToTranslate, string key)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    var decodedString = WebUtility.HtmlDecode(textToTranslate);

                    System.Object[] body = new System.Object[] { new { Text = decodedString } };
                    var requestBody = JsonConvert.SerializeObject(body);
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri($"{this.config.CSEndpoints.TextTranslator}&to={ language }");
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                    var response = await httpClient.SendAsync(request);
                   
                    return response;
                }
            }
        }
    }
}
