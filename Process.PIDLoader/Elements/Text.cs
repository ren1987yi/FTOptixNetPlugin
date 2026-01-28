using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Process.PIDLoader.Elements
{
    public class Text : IElement
    {

        private string _text;
        private Vector3 _position;
        private double _rotation;

        public string Value { get => _text; set => _text = value; }
        public Vector3 Position { get => _position;set =>  _position = value; }
        public double Rotation { get =>  _rotation; set => _rotation = value; }

        public string ElementTypeName { get => ElementType.Text.ToString(); }

        public string Summary => Value;

        public Text(string text,Vector3 position,double rotation)
        {
            _text = text;
            _position = position;
            _rotation = rotation;
        }

        public Text()
        {

        }
    }
}
