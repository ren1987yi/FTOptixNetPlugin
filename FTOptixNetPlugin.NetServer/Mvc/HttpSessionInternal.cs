using FTOptixNetPlugin.NetServer.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Web;

namespace FTOptixNetPlugin.NetServer.Mvc
{
    internal class HttpSessionInternal : HttpSession
    {
        private readonly HttpServerInternal _server;
        private readonly ILogger<HttpSessionInternal> _logger;
        public HttpSessionInternal(HttpServerInternal server) : base(server)
        {
            this._server = server;
            this._logger = server.ServiceProvider.GetRequiredService<ILogger<HttpSessionInternal>>();

        }

        protected override void OnReceivedRequest(HttpRequest request)
        {
            // Show HTTP request content
            //Console.WriteLine(request);

            try
            {
                string key = request.Url;

                var fakeurlstring = "http://127.0.0.1" + key;

                Uri _uri = new Uri(fakeurlstring, UriKind.Absolute);

                var query_parameters = HttpUtility.ParseQueryString(_uri.Query);

                var router_path = _uri.AbsolutePath;

                var req_method_type = HttpMethods.Empty;

                if (request.Method == "GET")
                {
                    req_method_type = HttpMethods.Get;
                }
                else if (request.Method == "POST")
                {
                    req_method_type = HttpMethods.Post;
                }
                else
                {
                    SendResponseAsync(Response.BadRequest());
                    return;
                }

                var mapper = _server.RouterMappers?.Where(r => r.MethodType == req_method_type && r.Router == Uri.UnescapeDataString(router_path.ToLower())).FirstOrDefault();
                if (mapper != null)
                {
                    var controller = _server.ServiceProvider.GetRequiredService(mapper.ControllerType) as Controller;
                    if (controller != null)
                    {
                        controller.SetContext(Request, query_parameters);
                        var res = mapper.Action.Invoke(controller, null) as IResult;
                        if (res == null)
                        {
                            SendResponseAsync(Response.NotFound());
                            this._logger.LogError("Request:{Request} Not Found,No Action \nBody:{Body}", Request.Url, Request.Body);
                        }
                        else
                        {

                            SendResponseAsync(res.MakeResponse(Response));
                        }
                    }
                    else
                    {
                        SendResponseAsync(Response.NotFound());
                        this._logger.LogError("Request:{Request} Not Found,No Controller \nBody:{Body}", Request.Url, Request.Body);
                    }
                }
                else
                {
                    if (key == "/" || key == "")
                    {
                        string defaultHtmlFile = null;
                        var dir = new DirectoryInfo(_server.DocumentPath);
                        if (dir.Exists)
                        {
                            defaultHtmlFile = dir.GetFiles("*.html").Where(c => c.Name.ToLower() == "index.html").Select(cc => cc.Name).FirstOrDefault();
                        }
                        else
                        {
                            SendResponseAsync(Response.NotFound());
                        }

                        if (defaultHtmlFile != null)
                        {
                            var filepath = Path.Combine(_server.DocumentPath, defaultHtmlFile);
                            var txt = File.ReadAllText(filepath);

                            HttpResponse httpResponse = new HttpResponse();
                            httpResponse.SetBegin(200);
                            httpResponse.SetContentType("text/html");

                            httpResponse.SetBody(txt);

                            SendResponseAsync(httpResponse);
                        }
                    }
                    else
                    {


                        SendResponseAsync(Response.NotFound());
                        this._logger.LogError("Request:{Request} Not Found ;Body:{Body}",Request.Url,Request.Body);
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                SendResponseAsync(Response.InternalServerError(ex.Message));
                this._logger.LogError("Request:{Request} Server Error ;Body:{Body}", Request.Url, Request.Body);
            }
        }

        protected override void OnReceivedRequestError(HttpRequest request, string error)
        {
            //Console.WriteLine($"Request error: {error}");
            SendResponseAsync(Response.InternalServerError(error));
            this._logger.LogError("Request:{Request} Error ;Message:{Body}", request.Url, error);

        }

        protected override void OnError(SocketError error)
        {
            base.OnError(error);
            this._logger.LogError("Socket Error:{error}",error.ToString());
        }
    }
}