using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public class Ok : IResult
    {
        public HttpResponse MakeResponse(HttpResponse response)
        {
            //throw new NotImplementedException();
            return response.MakeOkResponse();
        }
    }


    
}
