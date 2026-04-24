using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIKPlayer
{
    internal static class Program
    {


        


        internal static HttpService httpService;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AdminRelauncher.RelaunchIfNotAdmin();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var configure_file_name = "config.json";

 

 

            if (args.Length > 0)
            {
                configure_file_name = args[0];
            }

            //string exePath = Environment.ProcessPath;

            var dir = Environment.CurrentDirectory;

            var filepath = Path.Combine(dir, configure_file_name);
            Configure cfg;
            if (!File.Exists(filepath))
            {
                cfg = new Configure()
                {


                };
                var txt = JsonConvert.SerializeObject(cfg);
                File.WriteAllText(filepath, txt);
            }


            cfg = JsonConvert.DeserializeObject<Configure>(File.ReadAllText(filepath));
            App.Configuration = cfg;

            httpService = new HttpService(cfg.WebPort);
            
            httpService.StartHttpServer();

            App.Form = new MainForm();
            Application.Run(App.Form);

            httpService.CloseHttpServer();

        }
    }



    internal static class App
    {
        public static MainForm Form ;


        public static Configure Configuration ;
        public static WebCommand Command = new WebCommand();

    }
}


