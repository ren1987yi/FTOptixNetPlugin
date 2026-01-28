using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Process.PIDLoader.Elements
{
    public class AttributeDefine
    {
        public string Tag { get; set; }
        public string Value { get; set; }
        public AttributeDefine()
        {

        }

        public AttributeDefine(string tag,string value)
        {
            Tag = tag;
            Value = value;
        }
    }
}
