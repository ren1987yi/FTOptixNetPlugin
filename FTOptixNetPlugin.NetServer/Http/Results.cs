using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.NetServer.Http
{
    public static class Results
    {
        public static IResult Ok()
        {
            return new Ok();
        }

        public static IResult Text(string content)
        {
            return new TextResult(content);
        }

        public static IResult Redirect(string location)
        {
            return new Redirect(location);
        }


        public static IResult Html(string context)
        {
            return new HtmlResult(context);
        }

    }
}
