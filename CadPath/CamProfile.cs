using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CadPath
{
    public class CamProfile
    {
        public List<CamPoint> Points { get; set; } = new List<CamPoint>();
    }


    public class CamPoint
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
