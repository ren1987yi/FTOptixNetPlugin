

using System.Diagnostics;
using System.Numerics;

namespace CADExtract.Model
{
    public class InsertEntity : Entity
    {
        public string BlockName { get; set; } = string.Empty;
        public double Rotation { get; set; } = 0.0f;
        public Vector2 Position { get; set; }

        public InsertEntity() : base()
        {

        }

        public InsertEntity(ACadSharp.Entities.Insert data) : base()
        {
            this.ObjectType = EntityType.INSERT;
            BlockName = data.Block.Name;
            Rotation = data.Rotation;
            Position = new Vector2((float)data.InsertPoint.X, (float)data.InsertPoint.Y);

            this.Attributes.Clear();
            
            foreach(var attr in data.Attributes)
            {
                this.Attributes.Add(new CADExtract.Model.AttributeDefine() { Name = attr.Tag, Value = attr.Value });
            }
            

            

        }


        public InsertEntity(netDxf.Entities.Insert data) : base()
        {
            this.ObjectType = EntityType.INSERT;
            BlockName = data.Block.Name;
            Rotation = data.Rotation;
            Position = new Vector2((float)data.Position.X, (float)data.Position.Y);

            this.Attributes.Clear();

            foreach (var attr in data.Attributes)
            {
                this.Attributes.Add(new CADExtract.Model.AttributeDefine() { Name = attr.Tag, Value = attr.Value });
            }

        }
        public override string ToString()
        {
            //throw new NotImplementedException();
            return $"Block Name:{this.BlockName}; Position:{Position.X:f3}-{Position.Y:f3}; Rotation:{Rotation:f1}";
        }

    }
}
