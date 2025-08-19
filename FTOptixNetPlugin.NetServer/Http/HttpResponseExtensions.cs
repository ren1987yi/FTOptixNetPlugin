using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public static class HttpResponseExtensions
    {
        public static HttpResponse BadRequest(this HttpResponse response)
        {
            return response.MakeErrorResponse(400);
        }

        public static HttpResponse NotFound(this HttpResponse response,string content="",string contentType= "text/plain; charset=utf-8")
        {
            return response.MakeErrorResponse(404, content,contentType);
        }

        public static HttpResponse InternalServerError(this HttpResponse response, string content = "", string contentType = "text/plain; charset=utf-8")
        {
            return response.MakeErrorResponse(500);
        }

        public static HttpResponse Redirect(this HttpResponse response,string location)
        {
            response.Clear();
            response.SetBegin(301);
            response.SetHeader("Location", location);
            response.SetBody("");
            return response;
        }

    }
}
