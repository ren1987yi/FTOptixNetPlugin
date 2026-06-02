using DotNetWebServer;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InternalWebService.Controllers
{
    public class FileViewController: HttpController
    {

        [HttpMethod(HttpMethodType.GET, "home")]
        public IResult Home()
        {
            var f = Request.Url;

            Uri myUri = new Uri($"http://127.0.0.1{f}");
            string filepath = HttpUtility.ParseQueryString(myUri.Query).Get("f");

            var ext = Path.GetExtension(filepath);


            switch (ext.ToLower()) {
                case ".xlsx":
                    
                    return new Redirection($"/ExcelView/home?f={filepath}");

                    break;
                case ".js":
                case ".txt":
                case ".scl":
                case ".st":
                case ".cs":
                case ".c":
                case ".h":
                    return new Redirection($"/CodeEditor/home?f={filepath}");
                    break;
                default:
                    var txt = System.IO.File.ReadAllText(filepath);
                    return new TextResult(txt);
                    break;
            
            }



            return new TextResult($"file:{filepath}  Not Support");
        }


    }

    public class Redirection : Result, IResult
    {
        private string url;



        public string Url
        {
            get
            {
                return url;
            }
            private set
            {
                url = value;
            }
        }

      
        public Redirection(string url)
        {
            this.url = url;
        }

        public override HttpResponse MakeResponse(HttpResponse response)
        {
            response.Clear();
            response.SetBegin(302);
            response.SetHeader("Location", url);
            response.SetBody();
            return response;
            
        }
    }
}
