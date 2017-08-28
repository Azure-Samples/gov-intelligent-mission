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
    [Route("api/video")]
    public class VideoApiController : Controller
    {
        private IVideoManager videoManager;

        public VideoApiController(IVideoManager videoManager)
        {
            this.videoManager = videoManager;
        }

        [HttpPost("catalog-files")]
        public async Task<IActionResult> UploadNewCatalogFile(CatalogFileModel file, IFormFile uploadFile)
        {
            var catalogFile = await this.videoManager.UploadNewCatalogFile(file, uploadFile);
            return this.Ok(catalogFile);
        }

        [HttpGet("catalog-files")]
        public async Task<IActionResult> GetAllCatalogFiles()
        {
            var files = await this.videoManager.GetVideoCatalogFiles();
            return this.Ok(files);
        }

        [HttpGet("catalog-files/{id}")]
        public async Task<IActionResult> GetCatalogFile(string id)
        {
            var file = await this.videoManager.GetVideoCatalogFile(id);
            return this.Ok(file);
        }

        [HttpDelete("catalog-files/{id}")]
        public async Task<IActionResult> DeleteCatalogFile(string id)
        {
            await this.videoManager.DeleteCatalogFile(id);
            return this.NoContent();
        }

        [HttpPost("analysis/{id}")]
        public async Task<IActionResult> AnalyzeVideo(string id)
        {
            var response = await this.videoManager.DetectMotion(id);
            return this.Ok(response);
        }
    }
}
