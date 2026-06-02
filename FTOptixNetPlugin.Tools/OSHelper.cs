using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.Tools
{
    public class OSHelper
    {
        public static Process ExecuteExe(string filepath,string arguments,bool singleton)
        {
            if (singleton)
            {
                var processName = Path.GetFileNameWithoutExtension(filepath);
                var processes = Process.GetProcessesByName(processName);
                if(processes.Length > 0)
                {
                    return processes[0];
                }
            }

            var startInfo = new ProcessStartInfo()
            {
                FileName = filepath,
                Arguments = arguments,
                UseShellExecute = true,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                CreateNoWindow = true,
            };

            Process process = Process.Start(startInfo);
            return process;
        }
    }
}
