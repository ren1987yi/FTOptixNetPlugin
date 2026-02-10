using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCode2
{
    class Path
    {
        public int Index { get; set; }
        public int Count { get; set; } = 1;
        public string Name { get; set; }
        public List<Segment> Segments { get; private set; } = new List<Segment>();
    }

    class Segment
    {
        public int Index { get; set; }

        public bool VertexReverse { get; set; }

        public Entity Entity { get; set; }




    }

    internal class CADAnalysiser
    {
        public static void Go()
        {
            CadDocument dwg;

            var filepath = @"D:\Work\Projects\PID Loader\cad-polyline.dwg";
            using (DwgReader reader = new DwgReader(filepath))
            {
                dwg = reader.Read();

            }
            //CadDocument doc2 = new CadDocument();
            var paths = new List<Path>();

            foreach (var entity in dwg.Entities)
            {
                switch (entity.ObjectType)
                {
                    case ObjectType.SPLINE:
                        //parseSpline(entity as Spline);
                        break;
                    case ObjectType.LWPOLYLINE:
                    case ObjectType.POLYLINE_2D:
                        //parsePolyline(entity as LwPolyline);
                        //break;
                        //parsePolyline(entity as Polyline2D);

                        var path = paths.Where(p => p.Name == entity.Layer.Name).FirstOrDefault();
                        if(path == null)
                        {
                            path = new Path()
                            {
                                Name = entity.Layer.Name,
                            };
                            paths.Add(path);
                        }

                        var seg = new Segment()
                        {
                            Entity = entity,
                        };
                        path.Segments.Add(seg);


                        break;
                }
            }






            //var filepath = @"D:\Work\Projects\PID Loader\cad-2.dxf";
            //var dxf = netDxf.DxfDocument.Load(filepath);

            //Console.WriteLine(">>>>Polyline>>>>>>>>>>>");
            //foreach (var entity in dxf.Entities.Polylines2D)
            //{
            //    //entity

            //    var p1 = entity.Vertexes.FirstOrDefault().Position;
            //    var i = 0;
            //    foreach(var p in entity.Vertexes)
            //    {
            //        var x = p.Position.X - p1.X;
            //        var y = p.Position.Y - p1.Y;
            //        Console.WriteLine($"Point{i+1}      {x:f3},{y:f3}");
            //        i++;
            //    }
            //}

            //Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>");
            //Console.WriteLine("");
        }


        private static void parsePolyline(LwPolyline polyline)
        {
            //var p0 = polyline.Vertices.FirstOrDefault().Location;


            var p0 = CSMath.XY.Zero;

            Console.WriteLine(">>>>LWPolyline>>>>>>>>>>>");
            foreach(var p in polyline.Vertices)
            {
                //var p1 = p.Location - p0;
                var p1 = p.Location;
                Console.WriteLine($"P1 {p1.X:f3},{p1.Y:f3}");
            
            }

            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine("");
        }

        private static void parsePolyline(Polyline2D polyline)
        {
            //var p0 = polyline.Vertices.FirstOrDefault().Location;

            var p0 = CSMath.XYZ.Zero;


            Console.WriteLine(">>>>Polyline2D>>>>>>>>>>>");
            foreach (var p in polyline.Vertices)
            {
                //var p1 = p.Location - p0;
                var p1 = p.Location;
                Console.WriteLine($"P1 {p1.X:f3},{p1.Y:f3}");

            }

            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine("");
        }

        private static void parseSpline(Spline spline)
        {

            var l = new LwPolyline();

            var ps = spline.PolygonalVertexes(100);


            var p0 = ps.FirstOrDefault();
            Console.WriteLine(">>>>Polyline>>>>>>>>>>>");
            foreach (var p in ps)
            {
                var p1 = p - p0;
                Console.WriteLine($"P {p1.X:f3},{p1.Y:f3}");
                
            }

            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine("");



        }

    
    }
}
