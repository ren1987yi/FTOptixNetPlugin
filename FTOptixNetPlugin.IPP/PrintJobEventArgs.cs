using System;
using System.Collections.Generic;
using System.Text;

namespace FTOptixNetPlugin.IPP
{
    public class PrintJobEventArgs:EventArgs
    {
        public PrintJob Job { get; set; }
    }
}
