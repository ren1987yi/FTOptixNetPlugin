using Microsoft.Data.SqlClient;
using System.Data;

namespace FTOptixNetPlugin.DatabaseMaintenance
{
    public class MSSQL
    {

        string _server_or_ip = "."; 
        int _port = 1433;

        string _user = string.Empty;
        string _pwd = string.Empty;
        int _timeout = 30;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="server">mssql server_name or ip</param>
        /// <param name="port">mssql server port</param>
        /// <param name="user">user</param>
        /// <param name="password">password</param>
        /// <param name="timeout">connection timeout</param>
        public MSSQL(string server, int port, string user, string password, int timeout)
        {
            _server_or_ip = server;
            _port = port;
            _user = user;
            _pwd = password;
            _timeout = timeout;
        }

        /// <summary>
        /// database is exists?
        /// </summary>
        /// <param name="name">database name</param>
        /// <returns>-1:error,0:not exists,1:exists</returns>
        public int IsExistsDatabase(string name)
        {
            var connString = buildConnectionString("master");

            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();

                    string sql = @$"if exists(select * from sysdatabases where name='{name}')
begin
    select 1 as Result
end
else
begin
	select 0 as Result
end";
                    var cmd = new SqlCommand(sql, conn);
                    var result = cmd.ExecuteReader();
                    var dt = new DataTable();
                    dt.Load(result);

                    if (dt.Rows.Count > 0)
                    {
                        var r = Convert.ToInt32(dt.Rows[0]["Result"]);
                        return r;
                    }
                    else
                    {
                        return -1;
                    }



                }
            }
            catch (Exception ex)
            {
                return -1;
            }



        }

        /// <summary>
        /// get database file path
        /// </summary>
        /// <param name="name">database name</param>
        /// <returns> keypair : name ,filepath </returns>
        public Dictionary<string, string> GetDatabaseFilePath(string name)
        {
            var dc = new Dictionary<string, string>();

            var connString = buildConnectionString("master");

            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();

                    string sql = @$"select name,physical_name as CurrentLocation from sys.master_files where database_id=DB_ID('{name}')";
                    var cmd = new SqlCommand(sql, conn);
                    var result = cmd.ExecuteReader();
                    var dt = new DataTable();
                    dt.Load(result);

                    foreach (DataRow row in dt.Rows)
                    {
                        var fileName = row["name"].ToString();
                        var filePath = row["CurrentLocation"].ToString();
                        dc.Add(fileName, filePath);
                    }

                    return dc;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// get database all tables
        /// </summary>
        /// <param name="name">database</param>
        /// <returns></returns>
        public string[] GetTables(string name)
        {
            var connString = buildConnectionString(name);

            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();


                    string sql = @$"SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo'";
                    var cmd = new SqlCommand(sql, conn);
                    var reader = cmd.ExecuteReader();
                    var dt = new DataTable();
                    dt.Load(reader);

                    var names = from row in dt.AsEnumerable()
                                select row.Field<string>("TABLE_NAME");


                    return names.ToArray();



                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// delete tables data
        /// </summary>
        /// <param name="dbName">database name</param>
        /// <param name="tbNames">table names array</param>
        /// <returns></returns>
        public bool DeleteTables(string dbName, string[] tbNames)
        {
            var connString = buildConnectionString(dbName);

            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();

                    foreach (var name in tbNames)
                    {
                        string sql = @$"DELETE FROM [dbo].[{name}]";
                        var cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    return true;



                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// detach database
        /// </summary>
        /// <param name="name">database name</param>
        /// <returns></returns>
        public bool DetachDatabase(string name)
        {
            var connString = buildConnectionString("master");

            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();

                    string sql = @$"ALTER DATABASE [{name}] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();


                    sql = @$"EXEC master.dbo.sp_detach_db @dbname = N'{name}'";

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    return true;



                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// attach database
        /// </summary>
        /// <param name="name">database name</param>
        /// <param name="mdf_filepath">mdf file path</param>
        /// <param name="ldf_filepath">ldf file path</param>
        /// <returns></returns>
        public bool AttachDatabase(string name, string mdf_filepath, string ldf_filepath)
        {
            var connString = buildConnectionString("master");

            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();

                    string sql = @$"sp_attach_db @dbname, @mdf, @ldf";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@dbname", name);
                    cmd.Parameters.AddWithValue("@mdf", mdf_filepath);
                    cmd.Parameters.AddWithValue("@ldf", ldf_filepath);

                    cmd.ExecuteNonQuery();
                    return true;



                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// change database file name
        /// </summary>
        /// <param name="mdf_filepath">old mdf filepath</param>
        /// <param name="ldf_filepath">old ldf filepath</param>
        /// <param name="newName">change new name</param>
        /// <returns></returns>
        public (string mdf, string ldf) ChangeDatabaseFileName(string mdf_filepath, string ldf_filepath, string newName)
        {
            var folder = System.IO.Path.GetDirectoryName(mdf_filepath);
            var new_mdf = System.IO.Path.Join(folder, newName + ".mdf");


            folder = System.IO.Path.GetDirectoryName(ldf_filepath);
            var new_ldf = System.IO.Path.Join(folder, newName + "_log.ldf");

            return (new_mdf, new_ldf);
        }


        /// <summary>
        /// rename database file
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public (string mdf, string ldf) RenameDatabaseFile(string folder, string newName)
        {
            var new_mdf = Path.Join(folder, newName + ".mdf");
            var new_ldf = Path.Join(folder, newName + "_log.ldf");
            return (new_mdf, new_ldf);
        }


        /// <summary>
        /// 获取一个目录内的文件
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public string[] ListFolderFiles(string folder)
        {
            var connString = buildConnectionString("master");

            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();

                    using (var cmd = new SqlCommand("master.sys.xp_dirtree", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@directory", SqlDbType.NVarChar) { Value = folder });
                        cmd.Parameters.Add(new SqlParameter("@depth", SqlDbType.Int) { Value = 0 });
                        cmd.Parameters.Add(new SqlParameter("@file", SqlDbType.Int) { Value = 1 });

                        var read = cmd.ExecuteReader();
                        var dt = new DataTable();
                        dt.Load(read);
                        var filteredRows = from row in dt.AsEnumerable()
                                           where row.Field<int>("file") == 1
                                           select row.Field<string>("subdirectory");


                        return filteredRows.ToArray();
                    }

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// get system volumn status
        /// </summary>
        /// <returns></returns>
        public VolumnInfo[] GetVolumnStatus()
        {
            var connString = buildConnectionString("master");
            var sql = @"
SELECT DISTINCT
REPLACE(vs.volume_mount_point,':\','') AS Drive_Name ,
CAST(vs.total_bytes / 1024.0 / 1024 / 1024 AS NUMERIC(18,2)) AS Total_Space_GB ,
CAST(vs.available_bytes / 1024.0 / 1024 / 1024  AS NUMERIC(18,2)) AS Free_Space_GB
FROM    sys.master_files AS f
outer APPLY sys.dm_os_volume_stats(f.database_id, f.file_id) AS vs
";


            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();


                    using (var cmd = new SqlCommand(sql, conn))
                    {

                        var read = cmd.ExecuteReader();
                        var dt = new DataTable();
                        dt.Load(read);

                        var ss = from row in dt.AsEnumerable()
                                 select (
                                 new VolumnInfo(
                                    row.Field<string>("Drive_Name")
                                 , row.Field<decimal>("Total_Space_GB")
                                 , row.Field<decimal>("Free_Space_GB")
                                 ));

                        return ss.ToArray();
                    }





                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// get database file status
        /// </summary>
        /// <param name="name">database name</param>
        /// <returns></returns>
        public (float mdf_size, float ldf_size) GetDatabaseFileStatus(string name)
        {
            var connString = buildConnectionString(name);
            var sql = @"
SELECT 
    name,
    physical_name ,
    type_desc ,
    size * 8 / 1024 as Size
FROM sys.database_files;
";


            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();


                    using (var cmd = new SqlCommand(sql, conn))
                    {

                        var read = cmd.ExecuteReader();
                        var dt = new DataTable();
                        dt.Load(read);

                        var s = from row in dt.AsEnumerable()
                                where row.Field<string>("type_desc") == "ROWS"
                                select row.Field<int>("Size");
                        var mdf = s.FirstOrDefault();

                        s = from row in dt.AsEnumerable()
                            where row.Field<string>("type_desc") == "LOG"
                            select row.Field<int>("Size");

                        var ldf = s.FirstOrDefault();


                        return (Convert.ToSingle(mdf), Convert.ToSingle(ldf));
                    }





                }
            }
            catch (Exception ex)
            {
                return (0.0f, 0.0f);
            }
        }



        /// <summary>
        /// copy database file
        /// </summary>
        /// <param name="sourceFile">source file path</param>
        /// <param name="destFile">destination file path</param>
        /// <returns></returns>
        public bool CopyDatabaseFile(string sourceFile, string destFile)
        {

            try
            {
                if (FileExists(sourceFile))
                {


                    return CopyFile(sourceFile, destFile);


                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// copy file use mssql
        /// </summary>
        /// <param name="sourceFile">source file path</param>
        /// <param name="destFile">destination file path</param>
        /// <returns></returns>
        private bool CopyFile(string sourceFile, string destFile)
        {
            var connString = buildConnectionString("master");

            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();

                    string sql = @$"master.sys.xp_copy_file";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@source", SqlDbType.NVarChar) { Value = sourceFile });
                        cmd.Parameters.Add(new SqlParameter("@dest", SqlDbType.NVarChar) { Value = destFile });

                        cmd.ExecuteNonQuery();
                    }

                    return true;



                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// mssql server file exists
        /// </summary>
        /// <param name="filepath">file path</param>
        /// <returns></returns>
        public bool FileExists(string filepath)
        {
            var connString = buildConnectionString("master");

            try
            {
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();

                    using (var cmd = new SqlCommand("master.dbo.xp_fileexist", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@FilePath", SqlDbType.NVarChar) { Value = filepath });
                        cmd.Parameters.Add(new SqlParameter("@Exists", SqlDbType.Int) { Direction = ParameterDirection.Output });

                        var read = cmd.ExecuteReader();
                        var p = cmd.Parameters["@Exists"];
                        var rrrr = Convert.ToInt32(p.Value);

                        return rrrr == 1;

                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        
        
        /// <summary>
        /// build mssql connection string
        /// </summary>
        /// <param name="database_name">database name</param>
        /// <returns></returns>
        private string buildConnectionString(string database_name)
        {
            return @$"Server={_server_or_ip},{_port};Database={database_name};User ID={_user};Password={_pwd};TrustServerCertificate=True;Encrypt=False;Connect Timeout={_timeout};";
        }


    }


}
