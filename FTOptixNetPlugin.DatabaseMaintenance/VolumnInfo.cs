using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.DatabaseMaintenance
{
    public class VolumnInfo
    {
        public string Name { get; private set; }
        public double TotalSpace { get; private set; }
        public double UsedSpace => TotalSpace - FreeSpace;
        public double FreeSpace { get; private set; }

        public double UsedPersent => UsedSpace * 100 / TotalSpace;
        public double FreePersent => FreeSpace * 100 / TotalSpace;


        public VolumnInfo(string name,decimal total,decimal free)
        {
            Name = name;
            TotalSpace = Convert.ToDouble(total);
            FreeSpace = Convert.ToDouble(free);
                
        }


    }
}
