using HIKPlayer.HIK;
using NVRCsharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIKPlayer
{
    public partial class MainForm : Form
    {
        NVRInfo[] nvrs;

        PictureBox[] PlayerCanvas;

        public TableLayoutPanel Wall => LayoutPanel;

        public MainForm()
        {
            InitializeComponent();
            var m_bInitSDK = CHCNetSDK.NET_DVR_Init();


            LayoutPanel.RowCount = App.Configuration.Layout.RowCount;
            LayoutPanel.ColumnCount = App.Configuration.Layout.ColCount;
            LayoutPanel.RowStyles.Clear();
            LayoutPanel.ColumnStyles.Clear();
            for(var i = 0; i < App.Configuration.Layout.RowCount; i++)
            {
                LayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            }

            for (var i = 0; i < App.Configuration.Layout.ColCount; i++)
            {
                LayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            }


            //App.Configuration.Layout

            var count = LayoutPanel.RowCount * LayoutPanel.ColumnCount;
            PlayerCanvas = new PictureBox[count];
            for (var i = 0; i < count; i++)
            {
                var pic = new PictureBox();
                pic.BackColor = Color.Black;
                pic.Margin = new Padding(10);
                pic.Dock = DockStyle.Fill;
                LayoutPanel.Controls.Add(pic);
                PlayerCanvas[i] = pic;
            }

            this.Size = new Size(App.Configuration.Startup.Width, App.Configuration.Startup.Height);
            this.Location = new Point(App.Configuration.Startup.Left,App.Configuration.Startup.Top);
            //this.Left = App.Configuration.Startup.Left;
            //this.Top = App.Configuration.Startup.Top;
            //this.Width = App.Configuration.Startup.Width;
            //this.Height = App.Configuration.Startup.Height;




            Dictionary<Channel,NVRInfo> mapCh = new Dictionary<Channel, NVRInfo> ();

            nvrs = new NVRInfo[App.Configuration.Channels.Length];
            //login

            for (var i = 0; i < nvrs.Length; i++)
            {
                var ch = App.Configuration.Channels[i];
                nvrs[i] = new NVRInfo();
                HIKHelper.Login(ch.IP, (short)ch.Port, ch.User, ch.Password, ref nvrs[i]);

                mapCh.Add(ch, nvrs[i]);
            }








            foreach (var kv in mapCh)
            {
                var ch = kv.Key;
                var info = kv.Value;

                foreach (var screen in ch.Screens)
                {
                    var player = PlayerCanvas[screen.No];
                    var cameraNo = screen.ChNo;
                    HIKHelper.Preview(info,cameraNo,player);

                }
            }



            //priview
            //var ix = 0;
            //foreach(var nvr in nvrs)
            //{
            //    if(nvr.UserId < 0)
            //    {

            //    }
            //    else
            //    {
            //        HIKHelper.Preview(nvr, PlayerCanvas[ix]);
            //    }
            //     ix++;
            //}



        }







    }
}
