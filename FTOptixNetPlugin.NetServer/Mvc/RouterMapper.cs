using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Mvc
{
    internal class RouterMapper
    {
        private string _methodType;

        public string MethodType
        {
            get { return _methodType; }
            private set { _methodType = value; }
        }



        private string router;

        public string Router
        {
            get { return router; }
            private set { router = value; }
        }

        private Type controllerType;

        public Type ControllerType
        {
            get { return controllerType; }
            private set { controllerType = value; }
        }


        private MethodInfo action;

        public MethodInfo Action
        {
            get { return action; }
            private set { action = value; }
        }

        public RouterMapper(string method, string router, Type controllerType, MethodInfo action)
        {
            this._methodType = method;
            Router = router;
            ControllerType = controllerType;
            Action = action;
        }
    }
}
