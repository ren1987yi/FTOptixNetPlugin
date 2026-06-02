using DotNetWebServer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInternalServer(this WebApplication app,Configuration config) 
        {

            var s = app.Services.AddSingleton(config);
            app.UseStaticFile(new System.Collections.Generic.Dictionary<string, string>()
            {
                {"Access-Control-Allow-Origin","*" }
            });


            if (!Directory.Exists(config.UploadFileFolder))
            {
                Directory.CreateDirectory(config.UploadFileFolder);
            }

            if (!Directory.Exists(config.DownloadFileFolder))
            {
                Directory.CreateDirectory(config.DownloadFileFolder);
            }


            app.MapController(new[] { "InternalWebService.Controllers" });
            return s;
        }

        
        
    }
}
