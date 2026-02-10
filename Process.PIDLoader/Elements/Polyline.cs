using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Process.PIDLoader.Elements
{
    public class Polyline : IElement
    {
        private List<Vector3> _vertexes = new List<Vector3>();

        private double _thickness;

        private RGBA _color;
        public string Summary => $"Vertex Count:{_vertexes.Count}";
        public string ElementTypeName { get => ElementType.Line.ToString(); }
        public Polyline(IEnumerable<Vector3> points,double thickness,RGBA color)
        {
            _vertexes = new List<Vector3>();
            _vertexes.AddRange(points);
            _thickness = thickness;
            _color = color;
        }
        public Polyline()
        {

        }

        public List<Vector3> Vertexes { get => _vertexes; set => _vertexes = value; }
        public double Thickness { get => _thickness; set => _thickness = value; }
        public RGBA Color { get => _color; set => _color = value; }


      
    }
}
