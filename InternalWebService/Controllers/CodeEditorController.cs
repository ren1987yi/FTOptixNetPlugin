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
    public class CodeEditorController: HttpController
    {
        readonly string indexHtml = string.Empty;
        public CodeEditorController()
        {
            var filepath = Path.Combine(AppData.Instance.WebRoot, "codeeditor.html");
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

            var data = JsonConvert.DeserializeObject<FileRequestDto>(body);
            var path = data.filename;
            if (File.Exists(path))
            {

                var obj = new
                {
                    success = true,
                    message = string.Empty,
                    content = File.ReadAllText(path)
                };



                return new JsonResult(obj);
            }
            else
            {

                var value = AppData.Instance.LocalStorage.Get(path);
                if(value == null)
                {
                    var obj = new
                    {
                        success = false,
                        message = "file is not exists",
                        content = string.Empty
                    };

                    return new JsonResult(obj);

                }
                else
                {
                    var obj = new
                    {
                        success = true,
                        message = string.Empty,
                        content = value.Value,
                    };

                    return new JsonResult(obj);
                }
            }
        }







        [HttpMethod(HttpMethodType.POST)]
        public IResult Save()
        {

            var body = Request.Body;

            var data = JsonConvert.DeserializeObject<FileResponseDto>(body);


            if (AppData.Instance.LocalStorage.Exists(data.filename))
            {
                AppData.Instance.LocalStorage.SetValue(data.filename, data.content);
            }
            else
            {
                File.WriteAllText(data.filename, data.content);
            }




            var obj = new
            {
                success = true,
            };

            return new JsonResult(obj);

        }
    }
}
