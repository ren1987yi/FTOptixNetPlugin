using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADExtract.Model
{
    [Serializable]
    public class FileInfo
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }

        public Viewport Viewport { get; set; }

    }
}
