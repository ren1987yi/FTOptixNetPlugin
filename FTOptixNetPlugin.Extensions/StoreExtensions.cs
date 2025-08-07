using FTOptix.ODBCStore;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using UAManagedCore;

namespace FTOptixNetPlugin.Extensions
{
    public static class StoreExtensions
    {
        public static string GetStoreType(this Store store)
        {
            var t = store.GetType();
            if (t == typeof(SQLiteStore))
            {
                return "SQLite";
            }
            else if (t == typeof(ODBCStore))
            {
                var _s = store as ODBCStore;
                if (_s.Type == DBMSType.MSSQL)
                {
                    return "MSSQL";
                }
                else if (_s.Type == DBMSType.MySQL)
                {
                    return "MYSQL";
                }


            }

            return "Undefined";
        }


        public static bool TableExist(this Store store, string tablename)
        {
            var t = store.GetStoreType();
            switch (t.ToLower())
            {
                case "undefined":
                    throw new Exception("");
                case "sqlite":
                    var sql = $"select * from sqlite_master where type ='table' and name ='{tablename}'";
                    var res = store.Query(sql);
                    return res.Count() > 0;
                default:
                    return false;

            }
        }


        /// <summary>
		/// 查询记录
		/// </summary>
		/// <param name="store"></param>
		/// <param name="sql"></param>
		/// <returns>返回为可迭代字典,字段名称为Key</returns>
		public static IEnumerable<Dictionary<string, object>> Query(this Store store, string sql)
        {
            Object[,] ResultSet;
            String[] Header;

            store.Query(sql, out Header, out ResultSet);

            var result = new List<Dictionary<string, object>>();

            if (ResultSet.GetLength(0) > 0)
            {
                for (int i = 0; i < ResultSet.GetLength(0); i++)
                {
                    var dc = new Dictionary<string, object>();
                    result.Add(dc);
                    for (int j = 0; j < ResultSet.GetLength(1); j++)
                    {

                        try
                        {

                            var key = Header[j];
                            var value = ResultSet[i, j];
                            dc.Add(key, value);

                        }
                        catch
                        {
                            Log.Error("Favorites", "load error");
                            //ClearFavorites();
                        }
                    }

                }
            }
            else
            {
                Log.Info("StoreHelper", "query result empty");
                //ClearFavorites();

            }
            return result;

        }

        /// <summary>
		/// 插入一行记录
		/// </summary>
		/// <param name="store"></param>
		/// <param name="tablename"></param>
		/// <param name="names"></param>
		/// <param name="values"></param>
		public static void InsertOneRow(this Store store, string tablename, string[] names, object[] values)
        {
            object[,] vals = new object[1, values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                vals[0, i] = values[i];

            }


            store.Insert(tablename, names, vals);
        }

        /// <summary>
		/// 插入记录
		/// </summary>
		/// <param name="store"></param>
		/// <param name="tablename"></param>
		/// <param name="names"></param>
		/// <param name="values"></param>
		public static void Insert(this Store store, string tablename, string[] names, object[,] values)
        {
            try
            {

                store.Insert(tablename, names, values);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="store"></param>
        /// <param name="sql"></param>
        public static void ExecuteSql(this Store store, string sql)
        {
            String[] Header;
            Object[,] ResultSet;

            store.Query(sql, out Header, out ResultSet);

        }
    }
}
