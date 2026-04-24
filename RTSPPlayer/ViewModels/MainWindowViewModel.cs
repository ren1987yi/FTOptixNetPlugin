using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unosquare.FFME;
using Unosquare.FFME.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace RTSPPlayer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        static GridLength LAYOUT_ZERO = new GridLength(0);
        static GridLength LAYOUT_FULL = new GridLength(1, GridUnitType.Star);




        private string m_WindowTitle = string.Empty;
        private Visibility m_WindowVisibility = Visibility.Visible;
        private bool _M1Show = true;
        private DelegateCommand m_OpenCommand;

        private GridLength m_col1 = new GridLength(1, GridUnitType.Star);
        private GridLength m_col2 = new GridLength(1, GridUnitType.Star);
        private GridLength m_row1 = new GridLength(1, GridUnitType.Star);
        private GridLength m_row2 = new GridLength(1, GridUnitType.Star);

        private int m_WindowHeight = 600;
        private int m_WindowWidth = 800;
        private int m_WindowLeft = 0;   
        private int m_WindowTop = 0;


        MainWindow _w;

        public MainWindow Window => _w;












        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string WindowTitle
        {
            get => m_WindowTitle;
            set => SetProperty(ref m_WindowTitle, value);
        }
        public int WindowHeight
        {
            get => m_WindowHeight;
            set => SetProperty(ref m_WindowHeight, value);
        }


        public int WindowWidth
        {
            get => m_WindowWidth;
            set => SetProperty(ref m_WindowWidth, value);
        }


        public int WindowLeft
        {
            get => (int)m_WindowLeft;
            set => SetProperty(ref m_WindowLeft, value);
        }
        public int WindowTop
        {
            get => (int)m_WindowTop;
            set => SetProperty(ref m_WindowTop, value);
        }


        /// <summary>
        /// Gets or sets the close button visibility.
        /// </summary>
        public Visibility WindowVisibility
        {
            get => m_WindowVisibility;
            set => SetProperty(ref m_WindowVisibility, value);
        }




        
       

      
        public GridLength Col1
        {
            get => m_col1;
            set => SetProperty(ref m_col1, value);
        }
        public GridLength Col2
        {
            get => m_col2;
            set => SetProperty(ref m_col2, value);
        }
        public GridLength Row1
        {
            get => m_row1;
            set => SetProperty(ref m_row1, value);
        }
        public GridLength Row2
        {
            get => m_row2;
            set => SetProperty(ref m_row2, value);
        }




        public MainWindowViewModel() {

            WindowTitle = "HAHAHA";
        }

        Configure m_Cfg;
        public MainWindowViewModel(Configure cfg)
        {
            m_Cfg = cfg;
            WindowTitle = "HAHAHA";
            WindowLeft = m_Cfg.X;
            WindowTop = m_Cfg.Y;
            WindowWidth = m_Cfg.Width;
            WindowHeight= m_Cfg.Height;
            WindowVisibility = m_Cfg.StartupShow? Visibility.Visible: Visibility.Collapsed;
        }

        private Unosquare.FFME.MediaElement[] _allMEs;
        /// <summary>
        /// Called when application has finished loading.
        /// </summary>
        internal void OnApplicationLoaded(Window w)
        {
            _w = w as MainWindow;
            _allMEs = new Unosquare.FFME.MediaElement[] { _w.Media, _w.Media1, _w.Media2, _w.Media3 };



            //OpenCommand.Execute(null);
            OnInitStartup.Execute(null);

        }


        private DelegateCommand OnInitStartup => new DelegateCommand(async (p) =>
        {
            var count = 1;
            if (this.m_Cfg.StartupLayout == 0)
            {
                count = 1;
                Col1 = LAYOUT_FULL;
                Col2 = LAYOUT_ZERO;
                Row1 = LAYOUT_FULL;
                Row2 = LAYOUT_ZERO;
            }
            else
            {
                count = 4;
                Col1 = LAYOUT_FULL;
                Col2 = LAYOUT_FULL;
                Row1 = LAYOUT_FULL;
                Row2 = LAYOUT_FULL;
            }



            count = 4;



            for (var i = 0; i < count; i++)
            {
                //var uriString = string.Empty;
                //if (i % 2 == 0) {
                //    uriString = @"rtsp://admin:p-861100228@192.168.1.244:554/Streaming/Channels/101";
                //}
                //else
                //{
                //    uriString = @"rtsp://admin:p-861100228@192.168.1.244:554/Streaming/Channels/102";
                //}

                var uriString = m_Cfg.Urls[i];
                var target = new Uri(uriString);

                await _allMEs[i].Open(target);

            }
        });

       

        /// <summary>
        /// Gets the open command.
        /// </summary>
        /// <value>
        /// The open command.
        /// </value>
        public DelegateCommand OpenCommand => m_OpenCommand ??
            (m_OpenCommand = new DelegateCommand(async (a) =>
            {
                
                try
                {
                    
                    for(var i = 0; i < 4; i++)
                    {
                        //var uriString = string.Empty;
                        //if (i % 2 == 0) {
                        //    uriString = @"rtsp://admin:p-861100228@192.168.1.244:554/Streaming/Channels/101";
                        //}
                        //else
                        //{
                        //    uriString = @"rtsp://admin:p-861100228@192.168.1.244:554/Streaming/Channels/102";
                        //}

                        var uriString = m_Cfg.Urls[i];
                        var target = new Uri(uriString);

                        _allMEs[i].Open(target);

                    }
                    //    var uriString = a as string;
                    //if (string.IsNullOrWhiteSpace(uriString))
                    //    return;

                    //var m = _w.Media;
                    //var target = new Uri(uriString);
                    //if (target.ToString().StartsWith(FileInputStream.Scheme, StringComparison.OrdinalIgnoreCase))
                    //    await m.Open(new FileInputStream(target.LocalPath));
                    //else
                    
                    //await m.Open(target);
                    //await _w.Media1.Open(target);
                    //await _w.Media2.Open(target);
                    //await _w.Media3.Open(target);




                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        Application.Current.MainWindow,
                        $"Media Failed: {ex.GetType()}\r\n{ex.Message}",
                        $"{nameof(Unosquare.FFME.MediaElement)} Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error,
                        MessageBoxResult.OK);
                }
            }));

        public DelegateCommand Layout1_1Command => new DelegateCommand(async (p) =>
        {

            Col1 = LAYOUT_FULL;
            Col2 = LAYOUT_ZERO;
            Row1 = LAYOUT_FULL;
            Row2 = LAYOUT_ZERO;


            //Thread.Sleep(500);


            //for (var i = 1; i < 4; i++)
            //{

            //    await _allMEs[i].Close();

            //}

            //Thread.Sleep(2000);

            //for (var i = 0; i < 1; i++)
            //{
               
            //    var uriString = m_Cfg.Urls[i];
            //    var target = new Uri(uriString);

            //    await ms[i].Open(target);
            //    await ms[i].Play();
            //}

         
        });

        public DelegateCommand Layout2_2Command => new DelegateCommand(async (p) =>
        {


            Col1 = LAYOUT_FULL;
            Col2 = LAYOUT_FULL;
            Row1 = LAYOUT_FULL;
            Row2 = LAYOUT_FULL;
            //var ms = new Unosquare.FFME.MediaElement[] { _w.Media, _w.Media1, _w.Media2, _w.Media3 };

            //for (var i = 0; i < 4; i++)
            //{

            //    await _allMEs[i].Close();

            //}


            //await Task.Delay(500);

            //Thread.Sleep(500);


            //for (var i = 1; i < 4; i++)
            //{
            //    //var uriString = string.Empty;
            //    //if (i % 2 == 0) {
            //    //    uriString = @"rtsp://admin:p-861100228@192.168.1.244:554/Streaming/Channels/101";
            //    //}
            //    //else
            //    //{
            //    //    uriString = @"rtsp://admin:p-861100228@192.168.1.244:554/Streaming/Channels/102";
            //    //}

            //    var uriString = m_Cfg.Urls[i];
            //    var target = new Uri(uriString);

            //    await _allMEs[i].Open(target);
            //    await _allMEs[i].Play();
            //}
           
        });


        public DelegateCommand TranslateCommand => new DelegateCommand(async (p) =>
        {
            var pp = p as int[];
            WindowLeft = pp[0];
            WindowTop = pp[1];
            WindowWidth = pp[2];
            WindowHeight = pp[3];



        });


        public DelegateCommand ShutdownCommand => new DelegateCommand(async (p) => {

            App.Current.Shutdown();

        });
    }
}
