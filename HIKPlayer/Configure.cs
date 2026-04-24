using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIKPlayer
{
    internal class Configure
    {
        public int WebPort { get; set; } = 49001;


        public Startup Startup { get; set; } = new Startup();
        public ScreenLayout Layout { get; set; } = new ScreenLayout();
        public Channel[] Channels { get; set; } = new Channel[0];

    }


    internal class ScreenLayout
    {
        public int ColCount { get; set; }
        public int RowCount { get; set; }

    }


    internal class Channel
    {
        public string IP { get; set; }
        public int Port { get; set; }

        public string User { get; set; }
        public string Password { get; set; }

        public List<Screen> Screens { get; set; }
        
    }

    internal class Screen
    {
        public int No { get; set; }
        public int ChNo {  get; set; }
    }


    internal class Startup
    {
        public bool Show { get; set; } = true;
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

    }


}
