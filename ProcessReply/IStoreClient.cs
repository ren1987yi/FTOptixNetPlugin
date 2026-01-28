using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessReply
{
    internal interface IStoreClient
    {
        StoreClient_TE ClientType { get; }

        DataSetConfigure Configure { get; }


        bool Ping();

        Task<QueryValue[]> QuerySinglePoint(string[] fields, DateTime time ,int period = 1,ResolutionUnit_TE period_unit = ResolutionUnit_TE.Seconde);


    }
}
