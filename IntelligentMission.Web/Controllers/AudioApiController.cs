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
    [Route("api/audio")]
    public class AudioApiController : Controller
    {
        private AudioManager audioManager;

        public AudioApiController(AudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        /**
         * 1. Create Profile when person is first saved (generates identificationProfileId)
         * 2. Create Enrollment with a sample of their voice (text independent) - also upload to blob storage
         * 3. Perform recognition against list of (at most 10) identificationProfileIds
         **/

        [HttpPost("catalog-files")]
        public async Task<IActionResult> UploadNewCatalogFile(CatalogFileModel file, IFormFile uploadFile)
        {
            var catalogFile = await this.audioManager.UploadNewCatalogFile(file, uploadFile);
            return this.Ok(catalogFile);
        }

        [HttpGet("catalog-files")]
        public async Task<IActionResult> GetAllCatalogFiles()
        {
            var files = await this.audioManager.GetAudioCatalogFiles();
            return this.Ok(files);
        }

        [HttpGet("catalog-files/{audioId}")]
        public async Task<IActionResult> GetCatalogFile(string audioId)
        {
            var file = await this.audioManager.GetAudioCatalogFile(audioId);
            return this.Ok(file);
        }

        [HttpPost("{audioId}/recognize")]
        public async Task<IActionResult> Recognize(string audioId)
        {
            var result = await this.audioManager.Recognize(audioId);
            return this.Ok(result);
        }

        //Question: does this method belong in this controller?
        [HttpPost("{id}/enroll")]
        public async Task<IActionResult> Enroll(string id, IFormFile uploadFile)
        {
            var enrollment = await this.audioManager.Enroll(id, uploadFile);
            return this.Ok(enrollment);
        }
        
    }
}
