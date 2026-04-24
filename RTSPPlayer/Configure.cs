using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSPPlayer
{
    public class Configure
    {

        public int Port { get; set; } = 49001;
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 1024;
        public int Height { get; set; } = 768;

        public string[] Urls { get; set; } = new string[0];
        public bool StartupShow { get; set; } = true;

        public int StartupLayout { get; set; } = 0;

        public Configure() { }
    }
}
