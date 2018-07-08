using IntelligentMission.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<string> GetTranslation(string language, string textToTranslate)
        {
            var response = await this.GetTranslation(language, textToTranslate, this.config.Keys.TextTranslationKey);
            if (!response.IsSuccessStatusCode)
            {
                // Try just one more time. otherwise, something more significant is wrong.   
                
            }
            var result = await response.Content.ReadAsStringAsync();
            var parse = JsonConvert.DeserializeObject<dynamic>(result);
            string translatedtext = "";
            StringBuilder builder = new StringBuilder();

            foreach (dynamic trn in parse)
            {
                var translation = trn.translations;
                foreach (dynamic i in translation)
                {
                    string parsetranslation = i.text;
                    parsetranslation.Remove(0);
                    parsetranslation.Remove(parsetranslation.Length - 1);
                    translatedtext = parsetranslation;
                }
            }
            return translatedtext;
        }

        private async Task<HttpResponseMessage> GetTranslation(string language, string textToTranslate, string key)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    System.Object[] body = new System.Object[] { new { Text = textToTranslate } };
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
