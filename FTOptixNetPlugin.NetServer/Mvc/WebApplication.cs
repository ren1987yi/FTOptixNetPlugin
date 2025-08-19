using FTOptixNetPlugin.NetServer.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

//using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Net;
using System.Reflection;


namespace FTOptixNetPlugin.NetServer.Mvc
{
    public class WebApplication
    {
        public ServiceCollection Services { get => _services; }




        private readonly ServiceCollection _services = new ServiceCollection();
        private ServiceProvider _serviceProvider;

        private List<RouterMapper> _routerMappers = null;
        private IPAddress _address;
        private int _port;
        private string _prefix;
        private string _wwwroot;
        private HttpServerInternal _httpServer;
        private bool _enable_staticfile = false;
        private Dictionary<string, string> _staticfile_header_ext = null;
        private TimeSpan? _timeout;

        private ILogger<WebApplication> _logger;


        public WebApplication(IPAddress address, int port, string prefix, string document_path, TimeSpan? timeout = null, LogConfigure logConfig = null)
        {
            this._address = address;
            this._port = port;
            this._prefix = prefix;
            this._wwwroot = document_path;
            timeout ??= TimeSpan.FromSeconds(3600);

            this._timeout = timeout;



            _services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddNLog(logConfig?.Build());

            });

            //_logger.LogInformation("");
            //_services.AddSingleton(_logger);

        }

        public void UseStaticFile(Dictionary<string, string> header_ext = null)
        {
            _enable_staticfile = true;
            _staticfile_header_ext = header_ext;
        }

        /// <summary>
        /// 映射控制器
        /// </summary>
        /// <param name="allow_namespaces">循序的命名空间</param>
        public void UseRouting(string[] allow_namespaces = null)
        {
            _routerMappers = new List<RouterMapper>();

            //程序集
            var assList = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var ass in assList)
            {
                if (ass.FullName.StartsWith("System") || ass.FullName.StartsWith("Microsoft"))
                {
                    continue;
                }
                var typeInfos = ass.DefinedTypes;

                foreach (var typeInfo in typeInfos)
                {
                    //找 public , class ,controller
                    if ((typeInfo.IsPublic || typeInfo.IsNestedPublic) && typeInfo.IsClass && typeInfo.IsSubclassOf(typeof(Controller)))
                    {
                        var typefullname = typeInfo.FullName;
                        if (allow_namespaces != null)
                        {
                            var _found = false;

                            foreach (var prefix in allow_namespaces)
                            {
                                if (typefullname.StartsWith(prefix))
                                {
                                    _found = true;
                                    break;
                                }
                            }

                            if (!_found)
                            {
                                continue;
                            }
                        }

                        if (_services.Where(s => s.ServiceType.FullName == typeInfo.FullName).Count() > 0)
                        {
                            continue;
                        }

                        _services.AddTransient(typeInfo.UnderlyingSystemType);
                        //this._logger.LogInformation("Add Controller:{name}", typeInfo.FullName);
                        AddHttpHandler(typeInfo.UnderlyingSystemType);
                    }
                }

                //serviceProvider = services.BuildServiceProvider();
            }
        }

        private void AddHttpHandler(Type type)
        {
            var controller = string.Empty;

            var attr = type.GetCustomAttribute<ControllerAttribute>();
            if (attr != null)
            {
                controller = attr.ControllerName;
            }
            else
            {
                var typename = type.Name;
                if (typename.ToLower().EndsWith("controller"))
                {
                    controller = typename.Substring(0, typename.Length - "controller".Length);
                }
                else
                {
                    controller = typename;
                }
            }

            foreach (var methodInfo in type.GetMethods())
            {
                var actionName = string.Empty;
                var methodtype = HttpMethods.Empty;
                var mattr = methodInfo.GetCustomAttribute<RouteAttribute>();
                if (mattr != null)
                {
                    if (mattr.MethodType == HttpMethods.Get)
                    {
                        if (string.IsNullOrWhiteSpace(mattr.RouterPath))
                        {
                            actionName = methodInfo.Name;
                        }
                        else
                        {
                            actionName = mattr.RouterPath;
                        }
                        methodtype = HttpMethods.Get;
                    }
                    else if (mattr.MethodType == HttpMethods.Post)
                    {
                        if (string.IsNullOrWhiteSpace(mattr.RouterPath))
                        {
                            actionName = methodInfo.Name;
                        }
                        else
                        {
                            actionName = mattr.RouterPath;
                        }
                        methodtype = HttpMethods.Post;
                    }
                    else
                    {
                        //TODO no support http method
                        //_logger.LogWarning("Controller:{controller} Action:{action} set no support http method",controller,actionName);
                        continue;
                    }
                }


                if (!string.IsNullOrWhiteSpace(controller) && !string.IsNullOrWhiteSpace(actionName) && methodtype != HttpMethods.Empty)
                {
                    var router = "/" + controller.ToLower() + "/" + actionName.ToLower();

                    var mapper = new RouterMapper(methodtype, router, type, methodInfo);
                    _routerMappers.Add(mapper);
                    //this._logger.LogInformation("  Add Action:{method} {action}", methodtype, actionName);
                }
            }
        }

        public void Run()
        {
            _serviceProvider = _services.BuildServiceProvider();
            _logger = _serviceProvider.GetRequiredService<ILogger<WebApplication>>();

            _httpServer = new HttpServerInternal(_address, _port, _serviceProvider, _routerMappers);
            if (!string.IsNullOrWhiteSpace(_wwwroot))
            {
                if (_enable_staticfile)
                {
                    _httpServer.DocumentPath = _wwwroot;
                    _httpServer.AddStaticContent(_wwwroot, _prefix, "*.*", _timeout, _staticfile_header_ext);
                }
            }

            _httpServer.Start();


            _logger.LogInformation("Listen:0.0.0.0:{Port} ; WWWRoot:{WWWRoot}", _port, _wwwroot);

            foreach (var gg in _routerMappers.GroupBy(c => c.ControllerType))
            {
                _logger.LogInformation("Add Controller:{controller}", gg.Key.FullName);
                foreach (var mm in gg)
                {
                    _logger.LogInformation("  Add {method} {router}:{action} ", mm.MethodType, mm.Router, mm.Action.Name);
                }
            }
        }

        public void Stop()
        {
            _httpServer.Stop();
        }
    }
}