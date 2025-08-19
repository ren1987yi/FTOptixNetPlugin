using FTOptixNetPlugin.NetServer.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Mvc
{
    public class RouteAttribute:Attribute
    {
        private string _methodType;
        public string MethodType
        {
            get { return _methodType; }
            private set { _methodType = value; }
        }

        private string _routerPath = null;

        public string RouterPath
        {
            get { return _routerPath; }
            set { _routerPath = value; }
        }

        public RouteAttribute(string type = "GET", string router = null)
        {
            _methodType = type;
            _routerPath = router;
        }
    }
}
