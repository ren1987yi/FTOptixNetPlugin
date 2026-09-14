using FTOptixNetPlugin.PidRoutine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.PidRoutine
{
    public interface IServerConfiguration
    {
       

        Func<string, string,string,ICache,object> OpenFunction { get; set; }

        Func<SearchRequest,ICache,object> SearchFunction { get; set; }
        Func<object> ClearAllFunction { get; set; }

    }


    public class ServerConfiguration : IServerConfiguration
    {
        public Func<string, string, string, ICache, object> OpenFunction { get; set; }

        public Func<SearchRequest, ICache, object> SearchFunction { get; set; }
        public Func<object> ClearAllFunction { get; set; }
    }
}
