using ProcessReply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessReply
{
    public class QueryCompletedArgs:EventArgs
    {
        public IEnumerable<QueryValue> Results { get; private set; }

        public QueryCompletedArgs(IEnumerable<QueryValue> values)
        {
            Results = values.ToArray();
        }
    }
}
