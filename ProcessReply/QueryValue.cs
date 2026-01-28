using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessReply
{
    public struct QueryValue
    {
        public ulong Timestamp { get; private set; }
        public string Name { get; private set; }
        public object Value { get; private set;}

        public QueryValue(ulong time, string name, object value)
        {
            Timestamp = time;
            Name = name;
            Value = value;
        }
    }
}
