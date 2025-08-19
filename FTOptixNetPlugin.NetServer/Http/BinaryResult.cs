using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public class BinaryResult : IResult
    {
        private byte[] _content;
        private string _contentType;
        public BinaryResult(byte[] content,string contentType)
        {
            _content = content;
            _contentType = contentType;
        }
        public HttpResponse MakeResponse(HttpResponse response)
        {
            //throw new NotImplementedException();
            return response.MakeGetResponse(_content,_contentType);
        }
    }
}
