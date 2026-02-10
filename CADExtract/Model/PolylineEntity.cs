using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADExtract.Model
{
    public class PolylineEntity:Entity
    {

        public VertexTable Vertices { get; set; } = new VertexTable();

        public PolylineEntity():base() {
        
            
        }


        public PolylineEntity(ACadSharp.Entities.Line data) :base()
        {
            this.ObjectType = EntityType.POLYLINE;

            this.Vertices.Add(new System.Numerics.Vector2((float)data.StartPoint.X, (float)data.StartPoint.Y));
            this.Vertices.Add(new System.Numerics.Vector2((float)data.EndPoint.X, (float)data.EndPoint.Y));


            this.Attributes.Clear();
           
        }

        public PolylineEntity(ACadSharp.Entities.MLine data) : base()
        {
            this.ObjectType = EntityType.POLYLINE;

            this.Vertices.Add(new System.Numerics.Vector2((float)data.StartPoint.X, (float)data.StartPoint.Y));
            //this.Vertices.Add(new System.Numerics.Vector2((float)data.EndPoint.X, (float)data.EndPoint.Y));

            foreach(var v in data.Vertices)
            {
                this.Vertices.Add(new System.Numerics.Vector2((float)v.Position.X, (float)v.Position.Y));
            }


            this.Attributes.Clear();

        }
        public PolylineEntity(ACadSharp.Entities.LwPolyline data) : base()
        {
            this.ObjectType = EntityType.POLYLINE;

            foreach (var v in data.Vertices)
            {
                this.Vertices.Add(new System.Numerics.Vector2((float)v.Location.X, (float)v.Location.Y));
            }


            this.Attributes.Clear();

        }
        public PolylineEntity(ACadSharp.Entities.Polyline2D data) : base()
        {
            this.ObjectType = EntityType.POLYLINE;

            foreach (var v in data.Vertices)
            {
                this.Vertices.Add(new System.Numerics.Vector2((float)v.Location.X, (float)v.Location.Y));
            }


            this.Attributes.Clear();

        }

        public PolylineEntity(ACadSharp.Entities.Polyline3D data) : base()
        {

            this.ObjectType = EntityType.POLYLINE;
            foreach (var v in data.Vertices)
            {
                this.Vertices.Add(new System.Numerics.Vector2((float)v.Location.X, (float)v.Location.Y));
            }


            this.Attributes.Clear();

        }



        public PolylineEntity(netDxf.Entities.Line data) : base()
        {

            this.ObjectType = EntityType.POLYLINE;
            this.Vertices.Add(new System.Numerics.Vector2((float)data.StartPoint.X, (float)data.StartPoint.Y));
            this.Vertices.Add(new System.Numerics.Vector2((float)data.EndPoint.X, (float)data.EndPoint.Y));


            this.Attributes.Clear();

        }


        public PolylineEntity(netDxf.Entities.MLine data) : base()
        {



            this.ObjectType = EntityType.POLYLINE;

            foreach (var v in data.Vertexes)
            {
                this.Vertices.Add(new System.Numerics.Vector2((float)v.Position.X, (float)v.Position.Y));
            }


            this.Attributes.Clear();

        }


        public PolylineEntity(netDxf.Entities.Polyline2D data) : base()
        {
            this.ObjectType = EntityType.POLYLINE;
            foreach (var v in data.Vertexes)
            {
                this.Vertices.Add(new System.Numerics.Vector2((float)v.Position.X, (float)v.Position.Y));
            }


            this.Attributes.Clear();

        }


        public PolylineEntity(netDxf.Entities.Polyline3D data) : base()
        {
            this.ObjectType = EntityType.POLYLINE;
            foreach (var v in data.Vertexes)
            {
                this.Vertices.Add(new System.Numerics.Vector2((float)v.X, (float)v.Y));
            }


            this.Attributes.Clear();

        }
        public override string ToString()
        {
            //throw new NotImplementedException();
            return $"Vertices Count:{this.Vertices.Count}";
        }
    }
}
