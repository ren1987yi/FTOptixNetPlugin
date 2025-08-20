using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Text.Json;
namespace SampleCode
{
    internal class Test_DynamicObject:DynamicObject
    {
        //override 
        public static void JsonToObject()
        {
            var txt = @"{
""a"":123,
""b"":[11,22,33],
""c"":{
    ""aa"":123
}

}";

            var data = System.Text.Json.JsonSerializer.Deserialize<ExpandoObject>(txt);
            foreach(var item in data)
            {
                if(item.Value is JsonElement)
                {
                    var v = (JsonElement)item.Value;
                    switch (v.ValueKind)
                    {
                        case JsonValueKind.Number:
                            break;
                        case JsonValueKind.Object:
                            break;
                        case JsonValueKind.Array:
                            var fff = v.EnumerateArray();
                            break;
                    }
                    
                }
            }
        }
    }
}
