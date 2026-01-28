using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonApp
{
    internal class DaemonObject
    {
        private Process _process;


        public DaemonObject(string execute_path)
        {




            _process.Exited += process_Exited;
        }

        private void process_Exited(object? sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void Start()
        {
        }


        public void Stop() { 
        
        
        }

    }
}
