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
    public class ExcelViewController : HttpController
    {

        readonly string indexHtml = string.Empty;
        public ExcelViewController()
        {
            var filepath = Path.Combine(AppData.Instance.WebRoot,"xsheet.html");
            indexHtml = File.ReadAllText(filepath);
        }

        [HttpMethod(HttpMethodType.GET,"home")]
        public IResult Home()
        {
            


            var html = indexHtml;
            return new TextResult(html, "text/html");
        }
        



        /// <summary>
        /// 获取数据，测试用
        /// </summary>
        /// <returns></returns>
        [HttpMethod(HttpMethodType.POST)]
        public IResult Load()
        {
            var body = Request.Body;

            var data = JsonConvert.DeserializeObject<FileRequestDto>(body);
            var path = data.filename;
            if (File.Exists(path))
            {

                var sheets = XSSheet.XSHelper.ExcelToXSSheets(path);



                return new JsonResult(sheets);
            }
            else
            {
                return new JsonResult(new object[] { });
            }
        }

        [HttpMethod(HttpMethodType.POST)]
        public IResult Save()
        {
            var body = Request.Body;
            var res = new
            {
                success = true,
            };
            return new JsonResult(res);
        }

    }
}
