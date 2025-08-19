using System.Collections.Specialized;

namespace FTOptixNetPlugin.NetServer.Mvc
{
    public class Controller
    {
        protected HttpRequest Request;

        protected NameValueCollection QueryParameters;

        public void SetContext(HttpRequest request, NameValueCollection query)
        {
            Request = request;
            QueryParameters = query;
        }
    }
}
