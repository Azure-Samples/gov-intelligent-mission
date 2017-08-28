using IntelligentMission.Web.Models;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Services
{
    public class ImageAnalyzer
    {
        private IMDbRepository repository;
        private FaceApiClient faceApi;
        private VisionApiClient visionApi;

        public ImageAnalyzer(FaceApiClient faceApi, VisionApiClient visionApi, IMDbRepository repository)
        {
            this.faceApi = faceApi;
            this.visionApi = visionApi;
            this.repository = repository;
        }

        public async Task<dynamic> ObjectIdentify(string imageId)
        {
            var file = await this.repository.GetCatalogFile(imageId, FileType.Image);
            var standardVisionResults = await this.visionApi.AnalyzeImage(file.BlobUri);

            return standardVisionResults;
        }

        public async Task<dynamic> ObjectIdentifyAndOcr(string imageId)
        {
            var file = await this.repository.GetCatalogFile(imageId, FileType.Image);
            var standardVisionResults = await this.visionApi.AnalyzeImage(file.BlobUri);
            var ocrResults = await this.visionApi.OcrImage(file.BlobUri);
            return new
            {
                visionResults = standardVisionResults,
                ocrResults = ocrResults
            };
        }

        public async Task<List<IdentifiedFace>> Identify(string imageId)
        {
            // 1. Get Catalog File so we can get Blob URL
            var file = await this.repository.GetCatalogFile(imageId, FileType.Image);
            
            // 2. Detect all faces
            var detectedFaces = await this.faceApi.Detect(file.BlobUri);

            // 3. See if we can identify any faces from the ones detected
            var faceIds = detectedFaces.Select(x => x.FaceId).ToArray();
            var identifiedPersons = await this.IdentifyAll(faceIds);

            // 4. If anyone identified, map them up with originally detected face
            var identifiedFaces = detectedFaces.Select(x => new IdentifiedFace { Face = x }).ToList();
            foreach (var identifiedPerson in identifiedPersons)
            {
                var identifiedFace = identifiedFaces.SingleOrDefault(x => x.Face.FaceId.ToString() == identifiedPerson.FaceId);
                identifiedFace.IdentifiedPerson = identifiedPerson;
            }

            return identifiedFaces;
        }

        private async Task<List<IdentifiedPerson>> IdentifyAll(Guid[] faceIds)
        {
            var personGroups = await this.faceApi.GetPersonGroups();
            var allResults = new List<IdentifiedPerson>();

            foreach (var group in personGroups)
            {
                var groupResults = await this.IdentifyAllForGroup(faceIds, group.PersonGroupId);
                allResults.AddRange(groupResults);
            }

            return allResults;
        }

        private async Task<List<IdentifiedPerson>> IdentifyAllForGroup(Guid[] faceIds, string personGroupdId)
        {
            var skipToken = 0;
            const int pageSize = 10;
            var faceIdsSegment = Enumerable.Empty<Guid>();
            var resultList = new List<IdentifiedPerson>();

            // Since we might have more than 10 faces, we have to page calls to the Identify API
            do
            {
                faceIdsSegment = faceIds.Skip(skipToken).Take(pageSize);
                if (faceIdsSegment.Any())
                {
                    var results = await IdentifyBatch(faceIdsSegment.ToArray(), personGroupdId);
                    resultList.AddRange(results);
                }
                skipToken += pageSize;

            } while (faceIdsSegment.Any());

            return resultList;
        }

        private async Task<List<IdentifiedPerson>> IdentifyBatch(Guid[] faceIds, string personGroupId)
        {
            if (faceIds.Length > 10)
            {
                throw new ArgumentOutOfRangeException("You can only send at most 10 Face IDs to the Identity API.");
            }

            try
            {
                var resultList = new List<IdentifiedPerson>();
                var results = await this.faceApi.Identify(personGroupId, faceIds);
                foreach (var identifyResult in results)
                {
                    Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                    if (identifyResult.Candidates.Length == 0)
                    {
                        Console.WriteLine("No one identified");
                    }
                    else
                    {
                        // MATCH FOUND - Get top 1 among all candidates returned
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var personDocument = this.repository.GetPersonByFacePersonId(candidateId.ToString());
                        resultList.Add(new IdentifiedPerson
                        {
                            FaceId = identifyResult.FaceId.ToString(),
                            Person = personDocument,
                            Confidence = identifyResult.Candidates[0].Confidence
                        });
                    }
                }
                return resultList;
            }
            catch (FaceAPIException ex)
            {
                Console.WriteLine($"**** Exception: {ex.ErrorCode}, {ex.ErrorMessage}");
                throw ex;
            }
        }
    }
}
