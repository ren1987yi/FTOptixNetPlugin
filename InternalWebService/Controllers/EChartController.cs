using DotNetWebServer;
using InternalWebService.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService.Controllers
{
    public class EChartController : HttpController
    {

        readonly string indexHtml = string.Empty;
        public EChartController()
        {
            var filepath = Path.Combine(AppData.Instance.WebRoot, "echart.html");
            indexHtml = File.ReadAllText(filepath);
        }


        [HttpMethod(HttpMethodType.GET, "home")]
        public IResult Home()
        {
            var html = indexHtml;
            return new TextResult(html, "text/html");
        }


        [HttpMethod(HttpMethodType.POST)]
        public IResult Load()
        {
            var body = Request.Body;

            var result = AppData.Instance.ControlActionDelegateManager.GetDelegate("EChart", "Load")?.Invoke(body);

            if(result != null)
            {
                if(result is string strResult)
                {
                    return new TextResult(strResult, "application/json");
                }
                else
                {

                    return new JsonResult(result);
                }
            }

            return new TextResult("{}", "application/json");

        }



        [HttpMethod(HttpMethodType.POST)]
        public  IResult GetData()
        {
            var body = Request.Body;

            var result = AppData.Instance.ControlActionDelegateManager.GetDelegate("EChart", "GetData")?.Invoke(body);

            if (result != null)
            {
                if (result is string strResult)
                {
                    return new TextResult(strResult, "application/json");
                }
                else
                {

                    return new JsonResult(result);
                }
            }

            return new TextResult("{}", "application/json");
        }
    }
}
