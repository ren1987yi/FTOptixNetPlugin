using FTOptixNetPlugin.NetServer;
using FTOptixNetPlugin.NetServer.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SampleCode
{

    public class WsApplication
    {
        public event EventHandler OnError;



        public ServiceCollection Services { get => _services; }



        private IPAddress _address;
        private int _port;
        private string _prefix;
        private string _wwwroot;

        private TimeSpan? _timeout;
        private ILogger<WsApplication> _logger;




        private readonly ServiceCollection _services = new ServiceCollection();

        private ServiceProvider _serviceProvider;
        private WsServerEx _wsServer;

        public WsApplication(IPAddress address, int port, string static_prefix, string document_path, TimeSpan? timeout = null, LogConfigure logConfig = null)
        {


            this._address = address;
            this._port = port;
            this._prefix = static_prefix;
            this._wwwroot = document_path;
            timeout ??= TimeSpan.FromSeconds(3600);

            this._timeout = timeout;



            _services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddNLog(logConfig?.Build());

            });

        }



        public void Start()
        {
            _serviceProvider = _services.BuildServiceProvider();
            _logger = _serviceProvider.GetRequiredService<ILogger<WsApplication>>();

            _wsServer = new WsServerEx(_address, _port, _serviceProvider);

        }


        public void Stop()
        {



            
        }


        protected virtual void ErrorHandle()
        {
            if(OnError != null)
            {
                OnError.Invoke(this,new ());
            }
        }





    }


    internal class WsServerEx : WsServer
    {

        public Action ErrorAction { get; set; }

        private readonly IServiceProvider _serviceProvider;
        public WsServerEx(IPAddress address, int port, IServiceProvider serviceProvider) : base(address, port)
        {
            _serviceProvider = serviceProvider;
        }

        public WsServerEx(string address, int port,IServiceProvider serviceProvider) : base(address, port)
        {
            _serviceProvider = serviceProvider;
        }


        protected override TcpSession CreateSession()
        {
            //return base.CreateSession();
            return new WsSessionEx(this,_serviceProvider);
        }

        protected override void OnError(SocketError error)
        {
            if(ErrorAction != null)
            {
                ErrorAction.Invoke();
            }
            
            base.OnError(error);
        }


    }


    internal class WsSessionEx : WsSession
    {
        private readonly IServiceProvider _serviceProvider;
        public WsSessionEx(WsServer server,IServiceProvider serviceProvider) : base(server)
        {
            _serviceProvider = serviceProvider;
        }




    }


}
