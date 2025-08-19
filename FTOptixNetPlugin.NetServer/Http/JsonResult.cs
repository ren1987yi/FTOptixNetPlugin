using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
namespace FTOptixNetPlugin.NetServer.Http
{
    public class JsonResult<TValue> : IResult
    {
        private TValue _value;
        private JsonSerializerOptions _options = null;
        public JsonResult(TValue value,JsonSerializerOptions options = null)
        {
            _value = value;
        }
        public HttpResponse MakeResponse(HttpResponse response)
        {
            //throw new NotImplementedException();
            return response.MakeGetResponse(JsonSerializer.Serialize(_value,_options),"application/json; charset=UTF-8");
        }
    }
}
