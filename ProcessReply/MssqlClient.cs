using Microsoft.Data.SqlClient;
using System.Data;

namespace ProcessReply
{
    public class MssqlClient : IStoreClient
    {
        DataSetConfigure _configure;

        int _timeout = 30;

        string connectionString => @$"Server={_configure.Server},{_configure.Port};Database={_configure.Database};User ID={_configure.User};Password={_configure.Password};TrustServerCertificate=True;Encrypt=False;Connect Timeout={_timeout};";


        public StoreClient_TE ClientType => StoreClient_TE.MSSQL;

        public DataSetConfigure Configure => _configure;

        public MssqlClient(DataSetConfigure configure)
        {
            _configure = configure;
        }

        public bool Ping()
        {
            //Logger.Log($"String: {connectionString}");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //Logger.Log($"ServerVersion: {connection.ServerVersion}");
                //Logger.Log($"State: {connection.State}");
                return connection.State == ConnectionState.Open;
            }
        }

        public async Task<QueryValue[]> QuerySinglePoint(string[] fields, DateTime time, int period = 1, ResolutionUnit_TE period_unit = ResolutionUnit_TE.Seconde)
        {

            var s_time = time;
            var e_time = time.AddSeconds(60);

            var sql = @$"SELECT *,[LocalTimestamp] as [TimeIndex] FROM {_configure.Table} WHERE [LocalTimestamp]>= @st AND [LocalTimestamp]<= @et ORDER BY [LocalTimestamp]";


            List<QueryValue> values = new List<QueryValue>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Create and prepare an SQL statement.

                    SqlParameter stParam = new SqlParameter("@st", SqlDbType.DateTime);
                    SqlParameter etParam =
                        new SqlParameter("@et", SqlDbType.DateTime);
                    stParam.Value = s_time;
                    etParam.Value = e_time;


                    command.Parameters.Add(stParam);
                    command.Parameters.Add(etParam);
                    command.CommandTimeout = 30;
                    // Call Prepare after setting the Commandtext and Parameters.
                    command.Prepare();



                    var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);


                    var dt = new DataTable();
                    dt.Load(reader);


                    if(dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        var ts_ns = new DateTimeOffset(((DateTime)row["TimeIndex"])).ToUnixTimeMilliseconds();

                        foreach(var field in fields)
                        {
                            var value = row[field];
                            values.Add(new QueryValue((ulong)ts_ns, field, value));
                        }
                    }
                   
                      
                }
                catch
                {
                    return null;
                }
            }

            return values.ToArray();

        }
    }
}
