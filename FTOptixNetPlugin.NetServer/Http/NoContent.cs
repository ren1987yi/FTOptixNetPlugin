using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public class NoContent : IResult
    {
        public HttpResponse MakeResponse(HttpResponse response)
        {
            return response.MakeOkResponse(204);
        }
    }
}
