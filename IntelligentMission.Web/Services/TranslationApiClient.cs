using IntelligentMission.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
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
            var accessToken = this.cache.Get<string>(tokenKey);
            if (accessToken == null)
            {
                await this.RefreshAccessToken();
            }
            accessToken = this.cache.Get<string>(tokenKey);

            var response = await this.GetTranslation(language, textToTranslate, accessToken);
            if (!response.IsSuccessStatusCode)
            {
                // Try just one more time. otherwise, something more significant is wrong.
                await this.RefreshAccessToken();
                accessToken = this.cache.Get<string>(tokenKey);
                response = await this.GetTranslation(language, textToTranslate, accessToken);
                
            }
            var result = await response.Content.ReadAsStringAsync();
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(result);
            var value = xmlDoc["string"].InnerText;
            return value;
        }

        private async Task<HttpResponseMessage> GetTranslation(string language, string textToTranslate, string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync($"{this.config.CSEndpoints.TextTranslator}?to=en&text={textToTranslate}");
                return response;
            }
        }

        private async Task RefreshAccessToken()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add(Constants.OcpSubscriptionKey, this.config.Keys.TextTranslationKey);
                var response = await httpClient.PostAsync(this.config.CSEndpoints.TokenApi, null);
                var accessToken = await response.Content.ReadAsStringAsync();
                this.cache.Set(tokenKey, accessToken);
            }
        }
    }
}
