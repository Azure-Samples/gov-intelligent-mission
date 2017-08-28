using IntelligentMission.Web.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static IntelligentMission.Web.Services.IMDbRepository;

namespace IntelligentMission.Web
{
    internal static class Extensions
    {
        #region DocDb Extensions

        public static async Task<Document> CreateDocumentAsync(this DocumentClient docClient, object document)
        {
            return await docClient.CreateDocumentAsync(GetDocCollectionUri(), document);
        }

        public static async Task<Document> CreateDocumentAsync(this DocumentClient docClient, DocCollection dc, object document)
        {
            return await docClient.CreateDocumentAsync(dc.ToDocCollectionUri(), document);
        }

        public static async Task<Document> UpdateDocumentAsync<T>(this DocumentClient docClient, DocCollection dc, T document) where T : IIdentifiableItem
        {
            return await docClient.ReplaceDocumentAsync(dc.ToDocUri(document.Id), document);
        }

        public static async Task<Document> UpsertDocumentAsync<T>(this DocumentClient docClient, DocCollection dc, T document) where T : IIdentifiableItem
        {
            return await docClient.UpsertDocumentAsync(dc.ToDocUri(document.Id), document);
        }

        public static async Task<Document> ReadDocumentAsync(this DocumentClient docClient, DocCollection dc, string docId)
        {
            return await docClient.ReadDocumentAsync(dc.ToDocUri(docId));
        }

        public static Uri ToDocCollectionUri(this DocCollection dc) => UriFactory.CreateDocumentCollectionUri(DocDbNames.DbName, collectionMap[dc]);

        public static Uri ToDocUri(this DocCollection dc, string docId) => UriFactory.CreateDocumentUri(DocDbNames.DbName, collectionMap[dc], docId);

        private static Uri GetDocCollectionUri() => UriFactory.CreateDocumentCollectionUri(DocDbNames.DbName, DocDbNames.CatalogFiles);
        private static Uri GetDocUri(string id) => UriFactory.CreateDocumentUri(DocDbNames.DbName, DocDbNames.CatalogFiles, id);

        

        private static Dictionary<DocCollection, string> collectionMap = new Dictionary<DocCollection, string>
        {
            [DocCollection.CatalogFiles] = "catalog-files",
            [DocCollection.People] = "people",
            [DocCollection.AnalysisResults] = "analysis-results",
            [DocCollection.Invalid] = "**invalid**"
        };

        #endregion

        //public static (bool IsAuthenticated, string Name) ToViewBag(this System.Security.Claims.ClaimsPrincipal principal)
        public static AppIdentity ToViewBag(this System.Security.Claims.ClaimsPrincipal principal)
        {
            //const string imUserGroup = "c27ad02c-d7c3-445d-ac75-cfe1c1e2e1a5";
            //const string imAdminGroup = "6e39e5a1-5d19-4d27-b86d-23c36d5a8561";

            //return (IsAuthenticated: principal.Identity.IsAuthenticated, Name: $"{principal.Claims.GetClaim(ClaimTypes.GivenName)} {principal.Claims.GetClaim(ClaimTypes.Name)}");

            return new AppIdentity
            {
                IsAuthenticated = principal.Identity.IsAuthenticated.ToLower(),
                Name = $"{principal.Claims.GetClaim(ClaimTypes.GivenName)} {principal.Claims.GetClaim(ClaimTypes.Surname)}",
                IsIMUser = principal.Claims.HasGroupClaim(AADGroups.ImUserGroup).ToLower(),
                IsIMAdmin = principal.Claims.HasGroupClaim(AADGroups.ImAdminGroup).ToLower()
            };
        }

        public static object ToAnonType(this System.Security.Claims.ClaimsPrincipal principal)
        {
            //const string imUserGroup = "c27ad02c-d7c3-445d-ac75-cfe1c1e2e1a5";
            //const string imAdminGroup = "6e39e5a1-5d19-4d27-b86d-23c36d5a8561";

            //return (IsAuthenticated: principal.Identity.IsAuthenticated, Name: $"{principal.Claims.GetClaim(ClaimTypes.GivenName)} {principal.Claims.GetClaim(ClaimTypes.Name)}");

            return new 
            {
                IsAuthenticated = principal.Identity.IsAuthenticated,
                Name = $"{principal.Claims.GetClaim(ClaimTypes.GivenName)} {principal.Claims.GetClaim(ClaimTypes.Surname)}",
                IsUser = principal.Claims.HasGroupClaim(AADGroups.ImUserGroup),
                IsAdmin = principal.Claims.HasGroupClaim(AADGroups.ImAdminGroup)
            };
        }

        public static bool IsValidIMUser(this ClaimsPrincipal principal)
        {
            return principal.Claims.HasGroupClaim(AADGroups.ImUserGroup);
        }

        private static bool HasGroupClaim(this IEnumerable<Claim> claims, string groupId)
        {
            return claims.Any(c => c.Type == "groups" && c.Value == groupId);
        }

        private static string GetClaim(this IEnumerable<Claim> claims, string claimType)
        {
            var claim = claims.FirstOrDefault(c => c.Type == claimType);
            return claim == null ? null : claim.Value;
        }

        private static string ToLower(this bool value) => value.ToString().ToLower();

        //TODO: move this to config
        private static class AADGroups
        {
            public const string ImUserGroup = "c27ad02c-d7c3-445d-ac75-cfe1c1e2e1a5";
            public const string ImAdminGroup = "6e39e5a1-5d19-4d27-b86d-23c36d5a8561";
        }
    }
}
