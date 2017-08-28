using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Models
{
    public class AppIdentity
    {
        public string IsAuthenticated { get; set; }
        public string Name { get; set; }
        public string IsIMUser { get; set; }
        public string IsIMAdmin { get; set; }
    }
}
