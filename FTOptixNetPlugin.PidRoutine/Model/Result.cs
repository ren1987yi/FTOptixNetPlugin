using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.PidRoutine.Model
{
    public abstract class ResultBase
    {
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }
    }


    public class NormalResult : ResultBase
    {
        
    }

    public class OpenResult : ResultBase
    {
        public object Data { get; set; }
    }
    public class SearchResult : ResultBase
    {
        public object Data { get; set; }
    }


}
