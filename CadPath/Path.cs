using ACadSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CadPath
{
    internal class Path
    {
        public int Index { get; set; }
        public int Count { get; set; } = 1;
        public string Name { get; set; }
        public List<Segment> Segments { get; private set; } = new List<Segment>();
    }

    internal class Segment
    {
        public int Index { get; set; }

        public bool VertexReverse { get; set; }

        //public Entity Entity { get; set; }

        public List<Vector2> Points { get; set; }


    }
}
