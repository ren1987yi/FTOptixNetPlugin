using DotNetWebServer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.PidRoutine
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPidRoutineServer(this WebApplication app,
            IServerConfiguration config,ICache cache)
        {

            var s = app.Services.AddSingleton(config);
            s = s.AddSingleton(cache);
            app.UseStaticFile(new System.Collections.Generic.Dictionary<string, string>()
            {
                {"Access-Control-Allow-Origin","*" }
            });

            app.MapController(new[] { "FTOptixNetPlugin.PidRoutine.Controllers" });
            return s;
        }

        public static string GetWebRoot()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var folder = Path.GetDirectoryName(path);

            return Path.Combine(folder, "PidRoutineAssets");
        }



        

    }
}
