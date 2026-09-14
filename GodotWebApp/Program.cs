using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;

namespace GodotWebApp
{
    public abstract class WebSocketHandler
    {
        public WebSocketConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual void OnConnected(WebSocket socket, string key)
        {
            //var ServerSocket = WebSocketConnectionManager.GetWebSocket(key);
            //if (ServerSocket != null)
            //{
            //    WebSocketConnectionManager.AddSocket();
            //    Console.WriteLine("已经存在当前的连接，断开。。");
            //}
            WebSocketConnectionManager.AddSocket(socket, key);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            Console.WriteLine("Socket 断开了");
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: bytes, offset: 0, count: bytes.Length), messageType: WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            try
            {
                await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
            }
            catch (Exception)
            {

            }

        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }
        /// <summary>
        /// 获取一些连接
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IEnumerable<WebSocket> GetSomeWebsocket(string[] keys)
        {
            foreach (var key in keys)
            {
                yield return WebSocketConnectionManager.GetWebSocket(key);
            }
        }

        /// <summary>
        /// 给一堆人发消息
        /// </summary>
        /// <param name="webSockets"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToSome(WebSocket[] webSockets, string message)
        {
            webSockets.ToList().ForEach(async a => { await SendMessageAsync(a, message); });
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next,
                                          WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            string Key = context.Request.Query["Key"];
            Console.WriteLine("连接人：" + Key);
            Key = Guid.NewGuid().ToString();
            _webSocketHandler.OnConnected(socket, Key);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocketHandler.OnDisconnected(socket);
                    return;
                }

            });

            //TODO - investigate the Kestrel exception thrown when this is the last middleware
            //await _next.Invoke(context);
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            try
            {
                var buffer = new byte[1024 * 4];

                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                           cancellationToken: CancellationToken.None);

                    handleMessage(result, buffer);
                }
            }
            catch (Exception ex)
            {
                //GsLog.E(ex.StackTrace);
                Console.WriteLine(ex.StackTrace);
            }

        }
    }
    public class WebSocketConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public int GetCount()
        {
            return _sockets.Count;
        }

        public WebSocket GetSocketById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }
        public WebSocket GetWebSocket(string key)
        {
            WebSocket _socket;
            _sockets.TryGetValue(key, out _socket);
            return _socket;

        }

        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }
        public void AddSocket(WebSocket socket, string key)
        {
            if (GetWebSocket(key) != null)
            {
                _sockets.TryRemove(key, out WebSocket destoryWebsocket);
            }
            _sockets.TryAdd(key, socket);
            //string sId = CreateConnectionId();
            //while (!_sockets.TryAdd(sId, socket))
            //{
            //    sId = CreateConnectionId();
            //}



        }

        public async Task RemoveSocket(string id)
        {
            try
            {
                WebSocket socket;

                _sockets.TryRemove(id, out socket);


                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);


            }
            catch (Exception)
            {

            }

        }

        public async Task CloseSocket(WebSocket socket)
        {
            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
    public static class WebSocketExtensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, PathString path, WebSocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }

    public class Program
    {

        static void setupMIME(WebApplication app)
        {


            app.UseStaticFiles(new StaticFileOptions()
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            });
        }



        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            int port = 49003;

            if(args.Length == 2)
            {
                if(args[0] == "-p")
                {
                    port = int.Parse(args[1]);
                }
            }

            Console.WriteLine($"Listen:{port}");


            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(System.Net.IPAddress.Any, port); // HTTP
              
            });



            // Add services to the container.
            //builder.Services.AddAuthorization();

            builder.Services.AddWebSocketManager();


            builder.Services.AddSingleton<AppData>();
            builder.Services.AddSingleton<GodotHandle>();

            var app = builder.Build();

            setupMIME(app);
            //app.UseStaticFiles();

            // Configure the HTTP request pipeline.



            app.UseWebSockets();


            //app.UseAuthorization();


            GodotHandle godot = app.Services.GetService<GodotHandle>();
            AppData data = app.Services.GetService<AppData>();




            app.MapWebSocketManager("/com", godot);

            app.MapPost("/update", async (HttpContext context) =>
            {
                string body;
                using (var reader = new StreamReader(context.Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }

                data.GodotUpdateData = body;
                StringContent c = new(
                data.EventData,
                Encoding.UTF8,
                "text/plain");
                //await godot.SendMessageToAllAsync(body);
                return data.EventData;
            });


            app.MapPost("/command", async (HttpContext context) =>
            {
                string body;
                using (var reader = new StreamReader(context.Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }
                await godot.SendMessageToAllAsync(body);
                //await godot.SendMessageToAllAsync(data.GodotUpdateData);
                return Results.Ok();
            });

            PollStockPricesAsync(godot, data);
            app.Run();
        }


        static async Task PollStockPricesAsync(GodotHandle godot, AppData data)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(50));
            float pos = 0.0f;



            while (await timer.WaitForNextTickAsync())
            {
                //if (data.GodotUpdateDataHasChanged)
                //{

                //    data.GodotUpdateDataHasChanged = false;
                //}
                await godot.SendMessageToAllAsync(data.GodotUpdateData);
            }
        }




    }


    public class AppData
    {

        public bool GodotUpdateDataHasChanged { get; set; } = false;

        private string _godotUpdateData = string.Empty;
        public string GodotUpdateData
        {
            get => _godotUpdateData; 
            set
            {

                _godotUpdateData = value;
                GodotUpdateDataHasChanged = true;
            }
        }


        private string _lastEventData = string.Empty;
        public string EventData {get=>_lastEventData; set=>_lastEventData=value;}

    }


    public enum SendPackageType
    {
        None = 0,
        UpdateData = 1,
        Function = 2,
        ChangeScene = 3,
        TrackPos = 4
    }



    class GodotHandle : WebSocketHandler
    {
        readonly AppData data;


        public GodotHandle(WebSocketConnectionManager handler,AppData data) : base(handler)
        {
            this.data = data;
        }



        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var txt = System.Text.Encoding.UTF8.GetString(buffer);
            Console.WriteLine("recv:" + txt);
            //throw new NotImplementedException();

            this.data.EventData = txt;

            await SendMessageToAllAsync("hello");
        }
    }

}
