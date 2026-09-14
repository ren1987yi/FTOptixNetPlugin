using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.PidRoutine.CadModel
{
    public class Polyline
    {
        public float[] Color { get; set; }

        public string LineType { get; set; }
        public long LongColor { get; set; }

        public List<List<object>> Points { get; set; } = new List<List<object>>();
    }
    public class Text
    {
        public float[] Color { get; set; }
        public object Position { get; set; }
        public string Value { get; set; }
        public double Height { get; set; }
        public double Rotate { get; set; }
    }


    public class Circle
    {
        public float[] Color { get; set; }
        public object Center { get; set; }
        public double Radius { get; set; }

    }

    public class Arc
    {
        public float[] Color { get; set; }
        public object Center { get; set; }
        public double Radius { get; set; }

        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
    }



    /// <summary>
    /// 可交互对象
    /// </summary>
    public class Interactive
    {
        public string Name { get; set; }
        public long Handle { get; set; }

        public object Position { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Text { get; set; }
        public double TextHeight { get; set; }

    }


    public class Highlight
    {
        public List<List<object>> PolyLines { get; set; } = new List<List<object>>();
        public List<Text> Texts { get; set; } = new List<Text>();
        public List<Circle> Circles { get; set; } = new List<Circle>();
        public List<Arc> Arcs { get; set; } = new List<Arc>();

        public byte[] Color { get; set; } = new byte[] { 0, 255, 0 };
        public double WidthPx { get; set; } = 3;
    }

    public class Page
    {
        public long Width { get; set; }
        public long Height { get; set; }


        public List<Polyline> PolyLines { get; set; } = new List<Polyline>();
        public List<Text> Texts { get; set; } = new List<Text>();
        public List<Circle> Circles { get; set; } = new List<Circle>();
        public List<Arc> Arcs { get; set; } = new List<Arc>();

        public List<Interactive> Interactives { get; set; } = new List<Interactive>();
    }
}
