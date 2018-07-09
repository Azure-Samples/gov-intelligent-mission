using IntelligentMission.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Controllers
{
    [Route("api/translate")]
    public class TranslationApiController : Controller
    {
        private ITranslationApiClient translationApi;
        private INewsProviderClient newsClient;


        public TranslationApiController(ITranslationApiClient translationApi, INewsProviderClient newsClient)
        {
            this.translationApi = translationApi;
            this.newsClient = newsClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetTranslation(string id)
        {
            dynamic item = this.newsClient.GetNewsItem(id);
            var text = (string)item.item.description;
            var title = (string)item.item.title;

            var translatedText = await this.translationApi.GetTranslation("en", text);
            var translatedTitle = await this.translationApi.GetTranslation("en", title);
            var result = new
            {
                id = id,
                title = translatedTitle,
                description = translatedText
            };
            return this.Ok(result);
        }
        
    }
}
