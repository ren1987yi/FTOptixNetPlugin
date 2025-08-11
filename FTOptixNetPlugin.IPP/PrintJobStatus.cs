using System;
using System.Collections.Generic;
using System.Text;

namespace FTOptixNetPlugin.IPP
{
    public enum PrintJobStatus
    {
        None = 0,
        Success = 10000,
        Running = 1,
        Error = -2,
        DocumentEmpty = -3,
        PrinterClientError = -4,
    }
}
