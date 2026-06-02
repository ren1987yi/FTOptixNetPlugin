using DotNetWebServer;
using InternalWebService.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using InternalWebService.Utils;
namespace InternalWebService.Controllers
{
    public class FileController : HttpController
    {
        private readonly string UploadFolder = string.Empty;
        private readonly string DownloadFolder = string.Empty;


        private readonly string uploadHtml = string.Empty;
        private readonly string downloadHtml = string.Empty;

        private Configuration cfg = null;

        public FileController(Configuration cfg)
        {

            uploadHtml = File.ReadAllText(Path.Combine(AppData.Instance.WebRoot, "upload.html"));
            downloadHtml = File.ReadAllText(Path.Combine(AppData.Instance.WebRoot, "download.html"));
            this.cfg = cfg;

            UploadFolder = cfg.UploadFileFolder;
            DownloadFolder = cfg.DownloadFileFolder;
        }


        [HttpMethod(HttpMethodType.GET,"home")]
        public IResult GetPage()
        {
           
            var type = Request.GetHeaderValue("PAGE-TYPE");

            switch (type.ToLower()) { 
                case "upload":
                    return GetUploadPage();

                case "download":
                    return GetDownloadPage();
                default:
                    return GetDownloadPage();
            }

        }


        private IResult GetUploadPage()
        {
            var html = uploadHtml;


            return new TextResult(html, "text/html");
        }


        private IResult GetDownloadPage()
        {
            var html = downloadHtml;


            return new TextResult(html, "text/html");
        }


        [HttpMethod(HttpMethodType.POST)]
        public IResult Upload()
        {
            //Log.Info(Request.Body);
            //IFormFile f;


            var m = Request.GetFile().GetAwaiter().GetResult();


            //m.AsFileSection().FileStream.CopyTo(new FileStream(Path.Combine(UploadFolder, m.AsFileSection().FileName), FileMode.Create));
            //var ff = m.AsFileSection();
            //var ddd = m.AsFormDataSection();

            var ext = Path.GetExtension(m.FileName);

            var file = $"RENYI_KK_{DateTime.Now:yyyyMMddHHmmssffffff}_{Guid.NewGuid()}{ext}";


            
            var path = Path.Combine(UploadFolder, file);

            using (var st = new FileStream(path, FileMode.Create))
            {
                m.Section.Body.CopyTo(st);
            }


            var res = new
            {
                success = true,
                file = path,
                name = m.FileName
            };
            return new JsonResult(res);
        }


        [HttpMethod(HttpMethodType.POST)]
        public IResult UploadTrigger()
        {
            //Log.Info(Request.Body);

            var txt = Request.Body;


            var obj = JsonConvert.DeserializeObject<FileNotifyDto>(txt);
            //if (obj != null)
            //{
            //    Log.Info("Trigger", $"{obj.Id} ; {obj.File}");
            //}
            
            if(cfg.OnUpload != null)
            {
                cfg.OnUpload.Invoke(obj.Id, obj.File,obj.Name);
            }

            var res = new
            {
                success = true,

            };
            return new JsonResult(res);
        }


        [HttpMethod(HttpMethodType.POST)]
        public IResult GetFileById()
        {
            var txt = Request.Body;
            var obj = JsonConvert.DeserializeObject<FileQueryDto>(txt);


            var attr = AppData.Instance.Attachments.Where(c => c.Id == obj.Id).FirstOrDefault();

            // TODO 根据ID 返回 下载文件的路径
            var res = new
            {
                url = attr == null ? string.Empty : attr.Url,
                name = attr == null ? string.Empty : attr.Name,
                success = true

            };
            return new JsonResult(res);

        }


      
    }
}
