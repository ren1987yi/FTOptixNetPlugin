namespace ProcessReply
{
    internal class InfluxDBClient : IStoreClient
    {
        DataSetConfigure _configure;

        readonly Dictionary<ResolutionUnit_TE, string> _unitMapper = new()
        {
            { ResolutionUnit_TE.Seconde,"s"},
            { ResolutionUnit_TE.Minute,"m"},
            { ResolutionUnit_TE.Hour,"h"},
            { ResolutionUnit_TE.Millisecond,"ms"},
        };

        public StoreClient_TE ClientType => StoreClient_TE.InfluxDB;

        public DataSetConfigure Configure => _configure;

        private string Host
        {
            get
            {
                return $"http://{_configure.Server}:{_configure.Port}";
            }
        }

        public InfluxDBClient(DataSetConfigure configure)
        {
            _configure = configure;
        }

        public async Task<QueryValue[]> QuerySinglePoint(string[] fields, DateTime time, int period = 1, ResolutionUnit_TE period_unit = ResolutionUnit_TE.Seconde)
        {
            //TODO 分辨率 要参数化

            var utc = time.ToUniversalTime();
            var s_utc = utc.AddSeconds(-10);
            var e_utc = utc.AddSeconds(10);

            var s_utc_string = s_utc.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var e_utc_string = e_utc.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var every_value = "1s";
            if (_unitMapper.TryGetValue(period_unit, out var _unit))
            {
                every_value = $"{period}{_unit}";
            }




            var s_time = new DateTimeOffset(time);
            var e_time = new DateTimeOffset(time.AddSeconds(1));

            var s_ts_ns = s_time.ToUnixTimeSeconds() * 1000000000;
            var e_ts_ns = e_time.ToUnixTimeSeconds() * 1000000000;


            //var query_fields = new List<string>();
            //foreach(var f in fields)
            //{
            //    query_fields.Add(@$"r[""_field""] == ""{f}""");
            //}

            //var _query_fields = string.Join(" or ", query_fields);


            var flux = $@"
from(bucket: ""{_configure.Database}"")
  |> range(start: {s_utc_string} ,stop: {e_utc_string})
  |> filter(fn: (r) => r[""_measurement""] == ""{_configure.Table}"")
  |> aggregateWindow(every: {every_value}, fn: last, createEmpty: false)
  |> pivot(
      rowKey:[""_time""],
      columnKey: [""_field""],
      valueColumn: ""_value""
      )
 |> filter(fn: (r)=> r[""Timestamp""] >= {s_ts_ns}
  and r[""Timestamp""] <= {e_ts_ns}
   )
  |> fill(usePrevious: true)
  |> sort(columns: [""_time""] ,desc:false)
  |> limit(n:1)
  |> yield(name: ""last"")





";

            List<QueryValue> values = new List<QueryValue>();

            using (var client = new InfluxDB.Client.InfluxDBClient(Host, _configure.Token))
            {


                var fluxTables = await client.GetQueryApi().QueryAsync(flux, _configure.Org);

                if (fluxTables.Count > 0)
                {
                    var tb = fluxTables.First();
                    if (tb.Records.Count > 0)
                    {
                        var record = tb.Records.First();
                        var ts_ns = record.GetValueByKey("Timestamp");
                        foreach (var field in fields)
                        {
                            var value = record.GetValueByKey(field);
                            values.Add(new QueryValue((ulong)ts_ns, field, value));
                        }
                    }
                }



            }


            return values.ToArray();



        }

        public bool Ping()
        {
            //throw new NotImplementedException();
            using (var client = new InfluxDB.Client.InfluxDBClient(Host, _configure.Token))
            {
                return client.PingAsync().WaitAsync(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
            }
        }
    }
}
