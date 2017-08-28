using IntelligentMission.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public interface ITextApiClient
    {
        Task<string> ExtractKeyPhrases(string text);
        Task<string> ExecuteSentimentAnalysis(string text);
    }

    public class TextApiClient : ITextApiClient
    {
        private const string baseUrl = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0";
        private IMConfig config;

        public TextApiClient(IMConfig config)
        {
            this.config = config;
        }

        public async Task<string> ExtractKeyPhrases(string text)
        {
            var request = CreateRequestBody(text);
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.PostAsync($"{baseUrl}/keyPhrases", request);
                var json = await response.Content.ReadAsStringAsync();
                return json;
            }
        }

        public async Task<string> ExecuteSentimentAnalysis(string text)
        {
            var request = CreateRequestBody(text);
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.PostAsync($"{baseUrl}/sentiment", request);
                var json = await response.Content.ReadAsStringAsync();
                return json;
            }
        }

        #region Private Methods

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(Constants.OcpSubscriptionKey, this.config.Keys.TextAnalyticsKey);
            return httpClient;
        }

        private static StringContent CreateRequestBody(string text)
        {
            var html = WebUtility.HtmlDecode(text);
            var rawText = Regex.Replace(html, @"<[^>]+>|&nbsp;", "").Trim();
            var sentences = rawText.Split('.');
            var docs = new List<object>();
            for (int i = 0; i < sentences.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(sentences[i]))
                {
                    docs.Add(new
                    {
                        id = i + 1,
                        language = "en",
                        text = sentences[i]
                    });
                }
            }

            var request = new
            {
                documents = docs
            };
            return request.ToStringContent();
        }

        #endregion
    }
}
