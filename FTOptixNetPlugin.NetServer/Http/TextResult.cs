using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public class TextResult : IResult
    {
        private string _content;
        public TextResult(string content)
        {
            _content = content;
        }
        public HttpResponse MakeResponse(HttpResponse response)
        {
            return response.MakeGetResponse(_content);
        }
    }
}
