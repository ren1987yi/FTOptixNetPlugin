using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADExtract.Model
{

    public interface IEntity
    {
        AttributeTable Attributes { get; set; }
        EntityType ObjectType { get; set; }
        string ObjectTypeName { get; }

        string ToString();

        string Summary { get; }
    }

    public abstract class Entity : IEntity
    {
        private AttributeTable _attrs = new AttributeTable();
        private EntityType _entityType = EntityType.None;


        public AttributeTable Attributes { get => _attrs; set => _attrs = value; }
        public EntityType ObjectType { get => _entityType; set => _entityType = value; }
        public string ObjectTypeName => ObjectType.ToString();

        public string Summary => ToString();

        public Entity()
        {

        }


        public abstract string ToString();
        

    }
}
