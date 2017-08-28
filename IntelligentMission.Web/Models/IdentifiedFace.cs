using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentMission.Web.Models
{
    public class IdentifiedFace //: Face
    {
        public Face Face { get; set; }
        //public IMPerson PersonInfo { get; set; }
        public IdentifiedPerson IdentifiedPerson { get; set; }
    }

    public class IdentifiedPerson
    {
        public double Confidence { get; set; }
        public IMPerson Person { get; set; }
        public string FaceId { get; set; }
    }
}
