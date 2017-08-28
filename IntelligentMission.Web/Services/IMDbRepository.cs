using IntelligentMission.Web.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public partial class IMDbRepository
    {
        private DocumentClient docClient;

        public IMDbRepository(DocumentClient docClient)
        {
            this.docClient = docClient;
        }

        public async Task InitializeDatabase()
        {
            await this.CreateDatabaseIfNotExistsAsync();
            await this.CreateCollectionIfNotExistsAsync(DocDbNames.CatalogFiles);
            await this.CreateCollectionIfNotExistsAsync(DocDbNames.People);
        }

        public async Task<Document> SaveAnalysisResultsDoc(object doc)
        {
            //dynamic dynamicDoc = doc;
            //string id = dynamicDoc.id;
            var document = await this.docClient.UpsertDocumentAsync(DocCollection.AnalysisResults.ToDocCollectionUri(), doc);
            return document;
        }

        public async Task<Document> GetAnalysisResultsDoc(string id)
        {
            try
            {
                var doc = await this.GetDoc(DocCollection.AnalysisResults, id);
                return doc;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Document> CreatePersonDoc<T>(T doc)
        {
            return await this.docClient.CreateDocumentAsync(DocCollection.People, doc);
        }

        public async Task<Document> UpdatePersonDoc<T>(T doc) where T : IIdentifiableItem
        {
            return await this.docClient.UpdateDocumentAsync(DocCollection.People, doc);
        }

        public async Task<IMPerson> GetPersonDoc(string id)
        {
            var doc = await this.GetDoc(DocCollection.People, id);
            return (IMPerson)(dynamic)doc;
        }

        public async Task<Document> GetDoc(DocCollection dc, string id)
        {
            var doc = await this.docClient.ReadDocumentAsync(dc, id);
            return doc;
        }

        public async Task<Document> CreateDoc<T>(T doc)
        {
            //var uri = GetDocCollectionUri();
            //var savedDoc = await this.docClient.CreateDocumentAsync(uri, doc);
            //return savedDoc;
            return await this.docClient.CreateDocumentAsync(doc);
            //return await this.docClient.CreateDocumentAsync(GetDocCollectionUri(), doc);
        }

        public async Task<List<IMPerson>> GetPersonsByGroup(string personGroupId)
        {
            IDocumentQuery<IMPerson> query = this.docClient.CreateDocumentQuery<IMPerson>(DocCollection.People.ToDocCollectionUri())
                .Where(x => x.FacePersonGroupId == personGroupId)
                .OrderBy(x => x.FirstName)
                .AsDocumentQuery();
            return await ExecuteFullQuery<IMPerson>(query);
        }

        public IMPerson GetPersonByFacePersonId(string facePersonId)
        {
            var person = this.docClient.CreateDocumentQuery<IMPerson>(DocCollection.People.ToDocCollectionUri())
                .Where(x => x.FacePersonId == facePersonId)
                .AsEnumerable()
                .SingleOrDefault();
            return person;
        }

        public IMPerson GetPersonBySpeakerIdProfileId(string speakerIdentificationProfileId)
        {
            var person = this.docClient.CreateDocumentQuery<IMPerson>(DocCollection.People.ToDocCollectionUri())
                .Where(x => x.SpeakerIdentificationProfileId == speakerIdentificationProfileId)
                .AsEnumerable()
                .SingleOrDefault();
            return person;
        }

        public List<string> GetSpeakerIdentificationProfileIds()
        {
            var ids = this.docClient.CreateDocumentQuery<IMPerson>(DocCollection.People.ToDocCollectionUri())
                .Where(x => x.SpeakerIdentificationProfileId != null)
                .Select(x => x.SpeakerIdentificationProfileId)
                .ToList();
            return ids;
        }

        private static async Task<List<T>> ExecuteFullQuery<T>(IDocumentQuery<T> query)
        {
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }
            return results;
        }

        public async Task<List<CatalogFile>> GetCatalogFiles(FileType fileType)
        {
            IDocumentQuery<CatalogFile> query = this.docClient.CreateDocumentQuery<CatalogFile>(GetDocCollectionUri(),
                new FeedOptions { MaxItemCount = -1 })
                .Where(x => x.FileType == fileType)
                .OrderBy(x => x.Name)
                .AsDocumentQuery();

            var results = new List<CatalogFile>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<CatalogFile>());
            }

            return results;
        }

        public async Task<CatalogFile> GetCatalogFile(string docId, FileType fileType)
        {
            var doc = await this.docClient.ReadDocumentAsync<CatalogFile>(GetDocUri(docId), 
                new RequestOptions { PartitionKey = new PartitionKey((int)fileType) });
            return doc;
        }

        public async Task DeleteCatalogFile(string docId, FileType fileType)
        {
            await this.docClient.DeleteDocumentAsync(GetDocUri(docId),
                new RequestOptions { PartitionKey = new PartitionKey((int)fileType) });
        }



        #region Private Members

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await this.docClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DocDbNames.DbName));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await this.docClient.CreateDatabaseAsync(new Database { Id = DocDbNames.DbName });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync(string collectionName)
        {
            try
            {
                await this.docClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DocDbNames.DbName, collectionName));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await this.docClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DocDbNames.DbName),
                        new DocumentCollection { Id = collectionName },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion



        private static Uri GetDocCollectionUri() => UriFactory.CreateDocumentCollectionUri("intelligent-mission", "catalog-files");
        private static Uri GetDocUri(string id) => UriFactory.CreateDocumentUri("intelligent-mission", "catalog-files", id);
    }
}
