using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.PidRoutine.Model
{
    public class SearchRequest
    {
        public string f { get; set; }
        public string a { get; set; }
        public string s { get; set; }
        public long[] ids { get; set; }
    }
}
