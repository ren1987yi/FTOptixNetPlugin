using FTOptix.Core;
using FTOptix.HMIProject;
using FTOptix.UI;
using System.Numerics;
using System.Reflection;
using UAManagedCore;
namespace FTOptixNetPlugin.Extensions
{

    internal struct BBox
    {
        public float Left { get; private set; } = float.MaxValue;
        public float Right { get; private set; } = float.MinValue;
        public float Top { get; private set; } = float.MaxValue;
        public float Bottom { get; private set; } = float.MinValue;

        public float Width => Right - Left;
        public float Height => Bottom - Top;

        public Vector2 LeftTop => _lefttop;
        public Vector2 RightBottom => _rightbottom;


        private Vector2 _lefttop = Vector2.Zero;
        private Vector2 _rightbottom = Vector2.Zero;

        public BBox()
        {

        }

        public BBox(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }


        public void ProcessPoint(float x, float y)
        {
            Left = Math.Min(Left, x);
            Right = Math.Max(Right, x);
            Top = Math.Min(Top, y);
            Bottom = Math.Max(Bottom, y);

            _lefttop.X = Left;
            _lefttop.Y = Top;
            _rightbottom.X = Right;
            _rightbottom.Y = Bottom;
        }

    }

    public class Painter
    {

        public static Item AddUIObject(IUANode parent,IUANode uiNode, Vector2 p, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"{uiNode.BrowseName}_" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            }
            var item = InformationModel.MakeObject(name, uiNode.NodeId) as Item;
            item.LeftMargin = p.X;
            item.TopMargin = p.Y;
            parent.Add(item);
            return item;
                
        }


        public static Label AddLabel(IUANode parent,Vector2 p,string text,string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Label_" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            }
            var label = InformationModel.MakeObject<Label>(name);

            label.Text = text;

            label.LeftMargin = p.X;
            label.TopMargin = p.Y;


            parent.Add(label);

            return label;
        }


        public static PolyLine AddLine(IUANode parent, Vector2 p1, Vector2 p2, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Line_" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            }
            var line = InformationModel.MakeObject<PolyLine>(name);

            var bbox = new BBox();
            bbox.ProcessPoint(p1.X, p1.Y);
            bbox.ProcessPoint(p2.X, p2.Y);


            var pp1 = p1 - bbox.LeftTop;
            var pp2 = p2 - bbox.LeftTop;

            line.Path = $"M {pp1.X:f6} {pp1.Y:f6} L {pp2.X:f6} {pp2.Y:f6}";

            line.LineThickness = 1;

            line.LeftMargin = bbox.Left;
            line.TopMargin = bbox.Top;
            line.Width = bbox.Width;
            line.Height = bbox.Height;
            parent.Add(line);
            return line;
        }

        public static PolyLine AddPolyline(IUANode parent,IEnumerable<Vector2> points,Color lineColor ,Color fillColor , string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "PolyLine_" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            }

            var line = InformationModel.MakeObject<PolyLine>(name);

            var bbox = new BBox();
            foreach (var p in points)
            {
                bbox.ProcessPoint(p.X, p.Y);
            }

            var npoints = new List<Vector2>();
            foreach(var p in points)
            {
                var pp = p - bbox.LeftTop;
                npoints.Add(pp);
            }

            var paths = new List<string>();
            var i = 0;
            foreach(var p in npoints)
            {
                if(i == 0)
                {
                    paths.Add($"M {p.X:f6} {p.Y:f6}");
                }
                else
                {
                    paths.Add($"L {p.X:f6} {p.Y:f6}");
                }
                i++;
            }


            line.Path = string.Join(" ", paths);
            Log.Info(MethodBase.GetCurrentMethod().Name, line.Path);

            line.LineThickness = 1;
            line.LineColor = lineColor;
            line.FillColor = fillColor;
            line.LeftMargin = bbox.Left;
            line.TopMargin = bbox.Top;
            line.Width = bbox.Width;
            line.Height = bbox.Height;
            parent.Add(line);
            return line;

        }



        public static Rectangle AddRectangle(IUANode parent, Vector2 topLeft, Vector2 size, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            }
            var rect = InformationModel.MakeObject<Rectangle>(name);
            rect.LeftMargin = topLeft.X;
            rect.TopMargin = topLeft.Y;
            rect.Width = size.X;
            rect.Height = size.Y;
           
           
            parent.Add(rect);
            return rect;
        }




    }
}
