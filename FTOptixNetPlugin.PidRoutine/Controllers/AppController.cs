using DotNetWebServer;
using FTOptixNetPlugin.PidRoutine.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.PidRoutine.Controllers
{
    public class AppController : HttpController
    {

        private readonly IServerConfiguration _config;
        private readonly ICache _cache;
        public AppController(IServerConfiguration config, ICache cache) 
        {
            _config = config;
            _cache = cache;
        }   


        [HttpMethod(HttpMethodType.POST)]
        public IResult RefreshCache()
        {
            _cache.Refresh();
            return new JsonResult(new NormalResult() { Success = true });
        }


        [HttpMethod(HttpMethodType.POST)]
        public IResult Open()
        {
            var url = Request.Url;
            var filepath = Utils.GetUrlQuery(url, "f");
            var area = Utils.GetUrlQuery(url, "a");
            var step = Utils.GetUrlQuery(url, "s");


            var result = new OpenResult();

            if (string.IsNullOrWhiteSpace(filepath) || string.IsNullOrWhiteSpace(area))
            {
                result.Success = false;
                result.ErrorMsg = "filepath or area is empty";
            }
            else
            {
                try
                {
                    
                    if(_config.OpenFunction != null)
                    {
                        var data = _config.OpenFunction(filepath, area ,step,_cache);

                        result.Success = true;

                        result.Data = data;
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMsg = "open function not implemented";
                    }


                }
                catch
                {
                    result.Success = false;
                    result.ErrorMsg = "open error";
                    
                }

            }



            return new JsonResult(result);

        }


        [HttpMethod(HttpMethodType.POST)]
        public IResult Search()
        {
            var r = new SearchResult();



            var txt = Request.Body;
            var req = JsonConvert.DeserializeObject<SearchRequest>(txt);
            if (req == null)
            {
                r.Success = false;
            }
            else
            {

                if (string.IsNullOrWhiteSpace(req.f) || string.IsNullOrWhiteSpace(req.a))
                {
                    r.Success = false;
                    r.ErrorMsg = "filepath or area is empty";
                }
                else
                {
                    try
                    {

                        if (_config.SearchFunction != null)
                        {
                            var data = _config.SearchFunction(req, _cache);

                            r.Success = true;
                            r.Data = data;
                        }
                        else
                        {
                            r.Success = false;
                            r.ErrorMsg = "search function not implemented";

                        }

                    }
                    catch
                    {
                        r.Success = false;
                        r.ErrorMsg = "search error";
                        
                    }

                }
            }
            

            return new JsonResult(r);
        }

        [HttpMethod(HttpMethodType.POST)]
        public IResult ClearAll()
        {
            var result = new NormalResult();

            try
            {
                if (_config.ClearAllFunction != null)
                {
                    _config.ClearAllFunction();
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.ErrorMsg = "clear all function not implemented";
                }
            }
            catch
            {
                result.Success = false;
                result.ErrorMsg = "clear all error";
            }

            return new JsonResult(result);
        }
    }
}
