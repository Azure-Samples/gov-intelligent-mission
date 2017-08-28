using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IntelligentMission.Web.Services
{
    public interface INewsProviderClient
    {
        Task<string> GetLatestNewsWorld();
        Task<string> GetLatestNewsChina();
        JObject GetNewsItem(string id);
    }

    public class NewsProviderClient : INewsProviderClient
    {
        private const string baseNewsFeedUrl = "http://ftr.fivefilters.org/makefulltextfeed.php?url=http%3A%2F%2Ffeeds.reuters.com%2FReuters%2FworldNews&max=3";
        private const string baseNewsFeedUrlChina = "http://ftr.fivefilters.org/makefulltextfeed.php?url=http%3A%2F%2Fcn.reuters.com%2FrssFeed%2FchinaNews&max=3";

        // The memory cache is for simple demo purposes. Use Redis for the "real thing".
        private IMemoryCache cache;

        public NewsProviderClient(IMemoryCache cache)
        {
            this.cache = cache;
        }

        private async Task<string> GetLatestNews(string url, bool translationRequired = false)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                var xml = await response.Content.ReadAsStringAsync();

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                var nodes = xmlDoc.SelectNodes("/rss/channel/item");
                foreach (XmlNode node in nodes)
                {
                    var idNode = xmlDoc.CreateElement("id");
                    idNode.InnerText = Guid.NewGuid().ToString();
                    node.AppendChild(idNode);
                    var translationRequiredNode = xmlDoc.CreateElement("translationRequired");
                    translationRequiredNode.InnerText = translationRequired.ToString().ToLower();
                    node.AppendChild(translationRequiredNode);

                    var jsonItem = JsonConvert.SerializeXmlNode(node);
                    this.cache.Set(idNode.InnerText, jsonItem);
                }
                return JsonConvert.SerializeXmlNode(xmlDoc);
            }
        }

        public async Task<string> GetLatestNewsWorld()
        {
            return await this.GetLatestNews(baseNewsFeedUrl);
        }

        public async Task<string> GetLatestNewsChina()
        {
            return await this.GetLatestNews(baseNewsFeedUrlChina, translationRequired: true);
        }

        public JObject GetNewsItem(string id)
        {
            this.cache.TryGetValue<string>(id, out var value);
            return JObject.Parse(value);
        }
    }
}
