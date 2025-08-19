using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public class Redirect : IResult
    {
        string _location;
        public Redirect(string location) {
            _location = location;
        }
        public HttpResponse MakeResponse(HttpResponse response)
        {

            
            return response.Redirect(_location);
            
        }
    }
}
