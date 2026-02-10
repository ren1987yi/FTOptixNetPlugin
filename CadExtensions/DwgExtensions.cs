using ACadSharp;
using ACadSharp.Entities;
using Svg;
using Svg.Pathing;
using System.Drawing;

namespace CadExtensions
{
    public static class DwgExtensions
    {

        

        private static ACadSharp.Color getColor(this Entity item)
        {
            if (item.Color.IsByLayer)
            {
                return item.Layer.Color;

            }else if(item.Color.IsTrueColor) 
            {
                return new ACadSharp.Color(item.Color.R, item.Color.G, item.Color.B);
            }
            else
            {
                return new ACadSharp.Color();
            }
        }


        public static string ConvertToSvg(this CadDocument dwg)
        {
            var svgDoc = new Svg.SvgDocument();

            CSMath.BoundingBox bbox = CSMath.BoundingBox.Null;

            
            for(var i = 0; i < dwg.Entities.Count; i++)
            {
                if(i == 0)
                {
                    bbox = dwg.Entities[i].GetBoundingBox();
                }
                else
                {
                    var _box = dwg.Entities[i].GetBoundingBox();

                    bbox = _box.Merge(bbox);
                }
            }



            var minLength = Math.Min(bbox.LengthX, bbox.LengthY);

            var _zoom = 100;

            if (minLength < 100)
            {
                _zoom = 100;
            }
            else if (minLength < 1000)
            {
                _zoom = 10;
            }
            else
            {
                _zoom = 1;
            }


            svgDoc.Width = (float)bbox.LengthX * _zoom;
            svgDoc.Height = (float)bbox.LengthY * _zoom;
            svgDoc.ViewBox = new Svg.SvgViewBox((float)bbox.Min.X * _zoom, (float)bbox.Min.Y * _zoom, (float)bbox.LengthX * _zoom, (float)bbox.LengthY * _zoom);


            var group = new SvgGroup();
            svgDoc.Children.Add(group);
            

            foreach (var entity in dwg.Entities)
            {
                switch (entity.ObjectType)
                {
                    case ObjectType.LWPOLYLINE:
                        var lwpoly = entity as LwPolyline;
                       
                        group.Children.Add(convertLwPolyline(lwpoly,_zoom));
                        break;
                    case ObjectType.POLYLINE_2D:
                       
                        group.Children.Add(convertPolyline2D(entity as Polyline2D, _zoom));
                        break;

                    case ObjectType.LINE:

                        group.Children.Add(convertLine(entity as Line, _zoom));
                        break;


                }

            }



            var stream = new MemoryStream();
            svgDoc.Write(stream);
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd(); 
        }
    
    
    
        public static SvgElement convertLwPolyline(LwPolyline lw,float zoom)
        {
           
            var _color = lw.getColor();


            var svgPath = new SvgPath();
            svgPath.PathData = new SvgPathSegmentList();
            svgPath.AddStyle("fill", "none", 0);

            svgPath.AddStyle("stroke", $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}", 0);
            svgPath.AddStyle("stroke-width", "1", 0);

            for (var i = 0; i < lw.Vertices.Count; i++)
            {
                var v = lw.Vertices[i];
                float x, y;
                if (i == 0)
                {
                    x = (float)v.Location.X * zoom;
                    y = (float)v.Location.Y * zoom * -1;

                    x = float.Parse(x.ToString("f3"));
                    y = float.Parse(y.ToString("f3"));
                    svgPath.PathData.Add(new SvgMoveToSegment(false
                        , new PointF(x, y))
                        );

                }
                else
                {
                    x = (float)v.Location.X * zoom;
                    y = (float)v.Location.Y * zoom * -1;
                    x = float.Parse(x.ToString("f3"));
                    y = float.Parse(y.ToString("f3"));
                    svgPath.PathData.Add(new SvgLineSegment(false
                        , new PointF(x, y))
                        );
                }
            }

            return svgPath;
        }

        public static SvgElement convertPolyline2D(Polyline2D lw, float zoom)
        {

            var _color = lw.getColor();


            var svgPath = new SvgPath();
            svgPath.PathData = new SvgPathSegmentList();
            svgPath.AddStyle("fill", "none", 0);

            svgPath.AddStyle("stroke", $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}", 0);
            svgPath.AddStyle("stroke-width", "1", 0);

            for (var i = 0; i < lw.Vertices.Count; i++)
            {
                var v = lw.Vertices[i];
                float x, y;
                if (i == 0)
                {
                    x = (float)v.Location.X * zoom;
                    y = (float)v.Location.Y * zoom * -1;

                    x = float.Parse(x.ToString("f3"));
                    y = float.Parse(y.ToString("f3"));
                    svgPath.PathData.Add(new SvgMoveToSegment(false
                        , new PointF(x, y))
                        );

                }
                else
                {
                    x = (float)v.Location.X * zoom;
                    y = (float)v.Location.Y * zoom * -1;
                    x = float.Parse(x.ToString("f3"));
                    y = float.Parse(y.ToString("f3"));
                    svgPath.PathData.Add(new SvgLineSegment(false
                        , new PointF(x, y))
                        );
                }
            }

            return svgPath;
        }


        public static SvgElement convertLine(Line lw, float zoom)
        {

            var _color = lw.getColor();


            var svgPath = new SvgPath();
            svgPath.PathData = new SvgPathSegmentList();
            svgPath.AddStyle("fill", "none", 0);

            svgPath.AddStyle("stroke", $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}", 0);
            svgPath.AddStyle("stroke-width", "1", 0);

            float x, y;

            x = (float)lw.StartPoint.X * zoom;
            y = (float)lw.StartPoint.Y * zoom * -1;

            x = float.Parse(x.ToString("f3"));
            y = float.Parse(y.ToString("f3"));
            svgPath.PathData.Add(new SvgMoveToSegment(false
                , new PointF(x, y))
                );


            x = (float)lw.EndPoint.X * zoom;
            y = (float)lw.EndPoint.Y * zoom * -1;
            x = float.Parse(x.ToString("f3"));
            y = float.Parse(y.ToString("f3"));
            svgPath.PathData.Add(new SvgLineSegment(false
                , new PointF(x, y))
                );

            return svgPath;
        }


    }
}
