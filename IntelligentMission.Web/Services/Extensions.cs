using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public static class Extensions
    {
        public static Guid ToGuid(this string value) => new Guid(value);

        public static StringContent ToStringContent(this object value) => new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");


        /// <summary>
        /// This enables us to bind directly to our POCO config object and avoid the IOptions<T> dependency eveywhere
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static TConfig ConfigurePOCO<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var config = new TConfig();
            configuration.Bind(config);
            services.AddSingleton(config);
            return config;
        }

        public static string StripHtmlMarkup(this string html)
        {
            return Regex.Replace(html, @"<[^>]+>|&nbsp;", "").Trim();
        }
    }
}
