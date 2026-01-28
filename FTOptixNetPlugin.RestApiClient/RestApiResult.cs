using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.RestApiClient
{
    public class RestApiResult<T>
    {
        public bool Success { get; private set; }
        public T Value { get; private set; }

        public RestApiResult(bool success, T value)
        {
            Success = success;
            Value = value;
        }
        
    }


    public class RestApiStringResult : RestApiResult<string>
    {
        public RestApiStringResult(bool success, string value) : base(success, value)
        {

        }
    }
}
