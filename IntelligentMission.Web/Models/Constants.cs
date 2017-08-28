using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Models
{
    public static class Constants
    {
        public static string OcpSubscriptionKey = "Ocp-Apim-Subscription-Key";
    }

    internal static class DocDbNames
    {
        public const string DbName = "intelligent-mission";
        public const string CatalogFiles = "catalog-files";
        public const string People = "people";
    }
}
