using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FTOptixNetPlugin.PidRoutine
{
    internal class Utils
    {
        public static string GetUrlQuery(string url,string parameter)
        {
            Uri myUri = new Uri($"http://127.0.0.1{url}");
            string pp = HttpUtility.ParseQueryString(myUri.Query).Get(parameter);
            return pp;
        }


     


    }
}
