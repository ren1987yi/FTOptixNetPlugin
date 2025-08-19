using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace FTOptixNetPlugin.NetServer.Mvc
{
    internal class HttpServerInternal : HttpServer
    {
        public IServiceProvider ServiceProvider => _serviceProvider;
        public List<RouterMapper> RouterMappers => _routerMappers;

        public string DocumentPath { get; set; }

        private readonly IServiceProvider _serviceProvider;
        private readonly List<RouterMapper> _routerMappers;

        private ILogger<HttpServerInternal> _logger;

        public HttpServerInternal(IPAddress address, int port, IServiceProvider serviceProvider, List<RouterMapper> routerMappers) : base(address, port)
        {
            this._serviceProvider = serviceProvider;
            this._routerMappers = routerMappers;
            this._logger = _serviceProvider.GetRequiredService<ILogger<HttpServerInternal>>();
        }

        protected override TcpSession CreateSession()
        { return new HttpSessionInternal(this); }

        protected override void OnError(SocketError error)
        {
            //Console.WriteLine($"HTTP session caught an error: {error}");
            _logger.LogError("HTTP session caught an error: {error}", error.ToString());
        }
    }
}