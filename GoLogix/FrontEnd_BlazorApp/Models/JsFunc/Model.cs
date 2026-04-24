using System.Text.Json;
using System.Text.Json.Serialization;
namespace FrontEnd_BlazorApp.Models.JsFunc
{
    public class JsConfig
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("funcs")]
        public List<JsFuncDef> Functions { get; set; } = new List<JsFuncDef>();

    }

    public class JsFuncDef
    {
        [JsonPropertyName("cmd")]
        public int Cmd { get; set; }
        [JsonPropertyName("func_name")]
        public string FunctionName { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("body")]
        public string FunctionBody { get; set; }
        [JsonPropertyName("args")]
        public List<ArgumentDef> Arguments { get; set; } = new List<ArgumentDef>();
        [JsonPropertyName("returns")]
        public List<ArgumentDef> Returns { get; set; } = new List<ArgumentDef>();
    }


    public class ArgumentDef
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("datatype")]
        public string DataType { get; set; } = "dint";

    }

}
