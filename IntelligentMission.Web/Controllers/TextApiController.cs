using IntelligentMission.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Controllers
{
    [Route("api/text")]
    public class TextApiController : Controller
    {
        private INewsProviderClient newsClient;
        private ITextApiClient textApi;
        private ITranslationApiClient translationApi;


        public TextApiController(INewsProviderClient newsClient, ITextApiClient textApi, ITranslationApiClient translationApi)
        {
            this.newsClient = newsClient;
            this.textApi = textApi;
            this.translationApi = translationApi;
        }

        [HttpGet("latest-news")]
        public async Task<IActionResult> GetLatestNews()
        {
            var worldNewsFeed = await this.newsClient.GetLatestNewsWorld();
            var chinaNewsFeed = await this.newsClient.GetLatestNewsChina();
            var news = new
            {
                worldNews = JObject.Parse(worldNewsFeed),
                chinaNews = JObject.Parse(chinaNewsFeed)
            };
            return this.Ok(news);
        }

        [HttpPost("latest-news/{id}/analyze")]
        public async Task<IActionResult> Analyze(string id)
        {
            dynamic item = this.newsClient.GetNewsItem(id);
            var itemText = (string)item.item.description;
            var translationRequired = (bool)item.item.translationRequired;
            if (translationRequired)
            {
                itemText = await this.translationApi.GetTranslation("en", itemText);
            }
            var keyPhraseResult = await this.textApi.ExtractKeyPhrases(itemText);
            var sentimentResult = await this.textApi.ExecuteSentimentAnalysis(itemText);

            var result = new
            {
                keyPhrases = JObject.Parse(keyPhraseResult),
                sentiment = JObject.Parse(sentimentResult)
            };
            return this.Ok(result);
        }
        
    }
}
