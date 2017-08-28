using IntelligentMission.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Controllers
{
    [Route("api/analysis-results")]
    public class AnalysisResultsApiController : Controller
    {
        private IMDbRepository repository;

        public AnalysisResultsApiController(IMDbRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var doc = await this.repository.GetAnalysisResultsDoc(id);
            return this.Ok(doc);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody]object results)
        {
            var doc = await this.repository.SaveAnalysisResultsDoc(results);
            return this.Ok(doc);
        }
        
    }
}
