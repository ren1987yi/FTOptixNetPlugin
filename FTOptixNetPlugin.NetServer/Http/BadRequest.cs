using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public class BadRequest : IResult
    {
        
        public HttpResponse MakeResponse(HttpResponse response)
        {
            //throw new NotImplementedException();
            return response.MakeErrorResponse(400);
        }
    }
}
