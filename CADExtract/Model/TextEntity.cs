using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ACadSharp.Entities;
namespace CADExtract.Model
{
    public class TextEntity:Entity
    {
        public string Value { get; set; }
        public double Rotation { get; set; } = 0.0f;
        public Vector2 Position { get; set; }


        public TextEntity() : base()
        {
            
        }

        public TextEntity(ACadSharp.Entities.MText data):base()
        {
            this.ObjectType = EntityType.TEXT;
            Value = data.Value;
            Rotation = data.Rotation;
            Position = new Vector2((float)data.InsertPoint.X, (float)data.InsertPoint.Y);

        }

        public TextEntity(ACadSharp.Entities.TextEntity data) : base()
        {
            this.ObjectType = EntityType.TEXT;
            Value = data.Value;
            Rotation = data.Rotation;
            Position = new Vector2((float)data.InsertPoint.X, (float)data.InsertPoint.Y);
        }

        public TextEntity(netDxf.Entities.MText data) : base()
        {
            this.ObjectType = EntityType.TEXT;
            Value = data.Value;
            Rotation = data.Rotation;
            Position = new Vector2((float)data.Position.X, (float)data.Position.Y);


        }

        public TextEntity(netDxf.Entities.Text data) : base()
        {
            this.ObjectType = EntityType.TEXT;
            Value = data.Value;
            Rotation = data.Rotation;
            Position = new Vector2((float)data.Position.X, (float)data.Position.Y);
        }

        public override string ToString()
        {
            //throw new NotImplementedException();
            return $"Text:{Value}; Position:{Position.X:f3}-{Position.Y:f3}; Rotation:{Rotation:f1}";
        }
    }
}
