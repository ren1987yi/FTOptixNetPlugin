using ACadSharp;
using ACadSharp.Entities;
using CadExtensions;
using Svg;
using Svg.Pathing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadPath
{
    internal class SvgHelper
    {
        public void FromDwg(CadDocument dwg)
        {

            var ss = dwg.ConvertToSvg();


            File.WriteAllText(@"D:\Work\Projects\PID Loader\ttt.svg", ss);


            var svgDoc = new Svg.SvgDocument();

            CSMath.BoundingBox bbox = new CSMath.BoundingBox(0, 0, 0, 0, 0, 0);

            foreach (var entity in dwg.Entities)
            {
                var _box = entity.GetBoundingBox();

                bbox = _box.Merge(bbox);
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
                        var poly = entity as LwPolyline;

                        var svgPath = new SvgPath();
                        svgPath.PathData = new SvgPathSegmentList();
                        svgPath.AddStyle("fill", "none", 0);

                        svgPath.AddStyle("stroke", "black", 0);
                        svgPath.AddStyle("stroke-width", "1", 0);

                        for (var i = 0; i < poly.Vertices.Count; i++)
                        {
                            var v = poly.Vertices[i];
                            float x, y;
                            if (i == 0)
                            {
                                x = (float)v.Location.X * _zoom;
                                y = (float)v.Location.Y * _zoom * -1;

                                x = float.Parse(x.ToString("f3"));
                                y = float.Parse(y.ToString("f3"));
                                svgPath.PathData.Add(new SvgMoveToSegment(false
                                    , new PointF(x, y))
                                    );

                            }
                            else
                            {
                                x = (float)v.Location.X * _zoom;
                                y = (float)v.Location.Y * _zoom * -1;
                                x = float.Parse(x.ToString("f3"));
                                y = float.Parse(y.ToString("f3"));
                                svgPath.PathData.Add(new SvgLineSegment(false
                                    , new PointF(x, y))
                                    );
                            }
                        }

                        group.Children.Add(svgPath);
                        break;

                }

            }


            svgDoc.Write(@"D:\Work\Projects\PID Loader\ttt11.svg");

        }
    }
}
