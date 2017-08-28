using IntelligentMission.Web.Models;
using IntelligentMission.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Controllers
{
    [Route("api/image")]
    public class ImageApiController : Controller
    {
        private IMDbRepository repository;
        private ImageAnalyzer imageAnalyzer;
        private IImageManager imageManager;

        public ImageApiController(IMDbRepository repository, ImageAnalyzer imageAnalyzer, IImageManager imageManager)
        {
            this.repository = repository;
            this.imageAnalyzer = imageAnalyzer;
            this.imageManager = imageManager;
        }

        [HttpPost("catalog-files")]
        public async Task<IActionResult> UploadNewCatalogFile(CatalogFileModel file, IFormFile uploadFile)
        {
            var catalogFile = await this.imageManager.UploadNewCatalogFile(file, uploadFile);
            return this.Ok(catalogFile);
        }

        [HttpGet("catalog-files")]
        public async Task<IActionResult> GetAllCatalogFiles()
        {
            var files = await this.repository.GetCatalogFiles(FileType.Image);
            return this.Ok(files);
        }

        [HttpGet("catalog-files/{id}")]
        public async Task<IActionResult> GetCatalogFile(string id)
        {
            var file = await this.repository.GetCatalogFile(id, FileType.Image);
            return this.Ok(file);
        }

        [HttpDelete("catalog-files/{id}")]
        public async Task<IActionResult> DeleteCatalogFile(string id)
        {
            await this.imageManager.DeleteCatalogFile(id);
            return this.NoContent();
        }


        [HttpPost("analysis/{id}/identify")]
        public async Task<IActionResult> Detect(string id)
        {
            var result = await this.imageAnalyzer.Identify(id);
            return this.Ok(result);
        }

        [HttpPost("object-analysis/{id}/identify")]
        public async Task<IActionResult> ObjectIdentify(string id)
        {
            var result = await this.imageAnalyzer.ObjectIdentifyAndOcr(id);
            return this.Ok(result);
        }

    }
}
