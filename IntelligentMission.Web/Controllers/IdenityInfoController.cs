using IntelligentMission.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Controllers
{
    [Route("api/identity-info")]
    public class IdenityInfoController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            if (false)
            {
                var identityInfo =  new
                {
                    Name = "Steve Michelotti",
                    IsAuthenticated = true,
                    IsAdmin = true,
                    IsUser = true
                };
                return this.Ok(identityInfo);
            }
            else
            {
                var user = this.User.Identity;
                //var identityInfo = this.User.ToViewBag();
                var identityInfo = this.User.ToAnonType();
                return this.Ok(identityInfo);
            }

        }
        
    }
}
