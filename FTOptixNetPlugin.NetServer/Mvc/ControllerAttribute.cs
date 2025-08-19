using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Mvc
{
    public class ControllerAttribute:Attribute
    {
        private string _controllerName;

        public string ControllerName
        {
            get { return _controllerName; }
            private set { _controllerName = value; }
        }

        public ControllerAttribute(string name = null)
        {
            _controllerName = name;
        }
    }
}
