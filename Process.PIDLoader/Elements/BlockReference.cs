using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Process.PIDLoader.Elements
{
    public class BlockReference: IElement
    {
        private string _blockName;
        private Vector3 _position;
        private float _rotation;
        private Vector3 _scale;
        private List<AttributeDefine> _attributes;


        


        


        public string ElementTypeName { get => ElementType.Insert.ToString();  }
        public string Summary => _blockName;
        public BlockReference(string blockName,Vector3 position,float rotation,Vector3 scale)
        {
            _blockName = blockName;
            _position = position;
            _rotation = rotation;
            _scale = scale;
        }
        public BlockReference()
        {

        }


        public string BlockName { get => _blockName; set => _blockName = value; }
        public Vector3 Position { get => _position; set => _position = value; }
        public float Rotation { get => _rotation; set => _rotation = value; }
        public Vector3 Scale { get => _scale; set => _scale = value; }

        public List<AttributeDefine> Attributes { get => _attributes; set => _attributes = value; }

    }
}
