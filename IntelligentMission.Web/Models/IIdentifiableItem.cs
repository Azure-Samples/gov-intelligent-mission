using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Models
{
    public interface IIdentifiableItem
    {
        string Id { get; set; }
    }
}
