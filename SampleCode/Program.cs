#define WEBSOCKET
//#define IPP
//#define WebServer
//#define JSON
//#define Protainer

#if IPP
using FTOptixNetPlugin.IPP;
#endif
#if WebServer
using FTOptixNetPlugin.NetServer.Http;
using FTOptixNetPlugin.NetServer.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
#endif

#if Protainer
using NetProtainerApi;
#endif

#if WEBSOCKET
using SampleCode.WebSocketTest;
using System.Net;
#endif

namespace SampleCode
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

#if IPP
            Test_IPP();
#endif



#if WebServer
            Test_WebServer();
#endif

#if JSON
            Test_DynamicObject.JsonToObject();
#endif


#if Protainer
            var key = "ptr_CcB2BTvVSC6t9S09Jofu6NVNNvqqHDsJ5RGQ4NkwS0A=";
            var cli = new Client("http://192.168.1.102:9000",key);
            var v = await cli.GetEndPoints();

            var cc = await cli.ListAllContainers(v.First());

            Console.ReadLine();
#endif


#if WEBSOCKET
            // WebSocket server port
            int port = 51200;
            if (args.Length > 0)
                port = int.Parse(args[0]);
            // WebSocket server content path
            string www = "./wwwroot/ws";
            if (args.Length > 1)
                www = args[1];

            Console.WriteLine($"WebSocket server port: {port}");
            Console.WriteLine($"WebSocket server static content path: {www}");
            //Console.WriteLine($"WebSocket server website: http://localhost:{port}/chat/index.html");

            Console.WriteLine();

            // Create a new WebSocket server
            var server = new SampleCode.WebSocketTest.ChatServer(IPAddress.Any, port);
            server.AddStaticContent(www, "","*.*",TimeSpan.FromSeconds(10),null);


            // Start the server
            Console.Write("Server starting...");
            server.Start();
            Console.WriteLine("Done!");

            Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            for (; ; )
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Restart the server
                if (line == "!")
                {
                    Console.Write("Server restarting...");
                    server.Restart();
                    Console.WriteLine("Done!");
                }

                // Multicast admin message to all sessions
                line = "(admin) " + line;
                server.MulticastText(line);
            }

            // Stop the server
            Console.Write("Server stopping...");
            server.Stop();
            Console.WriteLine("Done!");
#endif


            Console.WriteLine("bye bye");
        }




#if IPP
        static void Test_IPP()
        {
            var filepath = @"C:\Users\YRen6\OneDrive - Rockwell Automation, Inc\Documents\Rockwell Automation\FactoryTalk Optix\ProjectTemplates\FTOptix_template_EdgeProjectTemplate\ProjectFiles\pdfs\25317000001381147015.pdf";

            var client = new Client(new Uri("ipp://10.108.164.27:631/ipp/print"));
            client.PrintJobStatusChanged += Client_PrintJobStatusChanged;
            var job = client.PrintFile(filepath, false, string.Empty, string.Empty);

            if (job.Status >= 0)
            {
                Console.WriteLine("job created");
            }
            else
            {
                Console.WriteLine("job error");
                return;
            }

            Console.ReadKey();
            client.PrintJobStatusChanged -= Client_PrintJobStatusChanged;
        }
        private static void Client_PrintJobStatusChanged(object? sender, PrintJobEventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine(e.Job.Status);

        }
#endif


#if WebServer
        static void Test_WebServer()
        {

            //var config = new NLog.Config.LoggingConfiguration();

            //// Targets where to log to: File and Console
            //var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            //var logconsole = new NLog.Targets.ColoredConsoleTarget("logconsole");

            //// Rules for mapping loggers to targets            
            //config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logconsole);
            //config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);

            //// Apply config           
            //NLog.LogManager.Configuration = config;


            var www_root = "./wwwroot";
            //singleton object
            Dictionary<string, string> obj = new Dictionary<string, string>();
            //logging configuration
            var config = new LogConfigure();

            config.AddColordConsole(FTOptixNetPlugin.NetServer.Mvc.LogLevel.Trace, FTOptixNetPlugin.NetServer.Mvc.LogLevel.Fatal);
            config.AddFile(FTOptixNetPlugin.NetServer.Mvc.LogLevel.Trace, FTOptixNetPlugin.NetServer.Mvc.LogLevel.Fatal,"log.txt");

            //
            var app = new WebApplication(System.Net.IPAddress.Any, 49000, string.Empty, www_root, TimeSpan.FromSeconds(3600), config);
            //dependency injection 
            app.Services.AddSingleton(obj);
            //use routing
            app.UseRouting();
            //use static files
            app.UseStaticFile();
            //run web application
            app.Run();

            Console.ReadLine();
        }



        public class AAController : Controller
        {
            private ILogger<AAController> _logger;

            public AAController(ILogger<AAController> logger)
            {
                _logger = logger;
            }

            [Route(HttpMethods.Get)]
            public IResult T1()
            {
                _logger.LogInformation("OK");
                return Results.Text("HAHA");
            }


            [Route(HttpMethods.Get)]
            public IResult T2()
            {
                return Results.Redirect("./t1");
            }
        }

#endif

    }
}
