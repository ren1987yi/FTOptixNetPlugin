using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Process.PIDLoader.Elements
{
    public interface IElement
    {
        public string ElementTypeName { get; }


        public string Summary { get; }

    }
}
