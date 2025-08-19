using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public class NotFound : IResult
    {
        private string _content;

        public NotFound(string content)
        {
            _content = content;
        }

        public HttpResponse MakeResponse(HttpResponse response)
        {
            //throw new NotImplementedException();
            return response.MakeErrorResponse(404, _content);
        }
    }
}
