using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace Process.PIDLoader.Elements
{
    public class Block
    {
        public string Name { get; set; }
        public List<AttributeDefine> Attributes { get; set; } = new List<AttributeDefine>();

        public string BindName { get; set; } = string.Empty;
        public Block(string name)
        {
            Name = name;
        }
    }
}
