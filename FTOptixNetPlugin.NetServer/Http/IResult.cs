using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public interface IResult
    {
        HttpResponse MakeResponse(HttpResponse response);
    }
}
