using Microsoft.Extensions.DependencyInjection;
using RTSPPlayer.ViewModels;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Windows;
using Unosquare.FFME;

namespace RTSPPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        public App()
        {
            // Change the default location of the ffmpeg binaries (same directory as application)
            // You can get the 64-bit binaries here: https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-full-shared.7z
            Library.FFmpegDirectory = @"c:\ffmpeg" + (Environment.Is64BitProcess ? @"\x64" : string.Empty);

            // Multi-threaded video enables the creation of independent
            // dispatcher threads to render video frames. This is an experimental feature
            // and might become deprecated in the future as no real performance enhancements
            // have been detected.
            Library.EnableWpfMultiThreadedVideo = true; // !System.Diagnostics.Debugger.IsAttached; // test with true and false
        }

        /// <summary>
        /// Provides access to the root-level, application-wide VM.
        /// </summary>


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var configure_file_name = "config.json";

            var args = e.Args;

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
                cfg = new Configure() { 
                
                
                };
                var txt = JsonSerializer.Serialize(cfg);
                File.WriteAllText(filepath, txt );
            }

            
            cfg = JsonSerializer.Deserialize<Configure>(File.ReadAllText(filepath));


            



            
            FTOptixNetPlugin.NetServer.Mvc.WebApplication v = new FTOptixNetPlugin.NetServer.Mvc.WebApplication(System.Net.IPAddress.Any, cfg.Port, "", "");
            v.UseRouting(new string[] { "RTSPPlayer.WebApi" });



            var w = new MainWindow();
            var vm = new MainWindowViewModel(cfg);
            Current.MainWindow = w;
            Current.MainWindow.DataContext = vm;
            Current.MainWindow.Loaded += (snd, eva) => vm.OnApplicationLoaded(w);

            Current.MainWindow.Show();

            v.Services.AddSingleton(vm);



            // Pre-load FFmpeg libraries in the background. This is optional.
            // FFmpeg will be automatically loaded if not already loaded when you try to open
            // a new stream or file. See issue #242
            Task.Run(async () =>
            {
                try
                {
                    // Pre-load FFmpeg
                    await Library.LoadFFmpegAsync();

                    v.Run();
                }
                catch (Exception ex)
                {
                    var dispatcher = Current?.Dispatcher;
                    if (dispatcher != null)
                    {
                        await dispatcher.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show(MainWindow,
                                $"Unable to Load FFmpeg Libraries from path:\r\n    {Library.FFmpegDirectory}" +
                                $"\r\nMake sure the above folder contains FFmpeg shared binaries (dll files) for the " +
                                $"applicantion's architecture ({(Environment.Is64BitProcess ? "64-bit" : "32-bit")})" +
                                $"\r\nTIP: You can download builds from https://ffmpeg.org/download.html" +
                                $"\r\n{ex.GetType().Name}: {ex.Message}\r\n\r\nApplication will exit.",
                                "FFmpeg Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                            Current?.Shutdown();
                        }));
                    }
                }
            });
        }
    }

}
