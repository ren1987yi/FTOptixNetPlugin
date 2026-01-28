#define WEBSOCKET11
#define PROCESS_REPLAY11
#define SCRIBAN11
#define DXF

#if WEBSOCKET
using FTOptixNetPlugin.NetServer;
using SampleCode2.WebSocket_Demo;
#endif


#if PROCESS_REPLAY
using ProcessReply;
#endif

#if SCRIBAN
using Microsoft.Data.SqlClient;
using NLog.Targets;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
#endif

#if DXF
using NLog.Targets;
using Process.PIDLoader;
#endif

namespace SampleCode2
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if WEBSOCKET

            // WebSocket server address
            string address = "127.0.0.1";
            if (args.Length > 0)
                address = args[0];

            // WebSocket server port
            int port = 51200;
            if (args.Length > 1)
                port = int.Parse(args[1]);

            Console.WriteLine($"WebSocket server address: {address}");
            Console.WriteLine($"WebSocket server port: {port}");

            Console.WriteLine();

            Console.WriteLine("sleep 5s...");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("sleep done");
            // Create a new TCP chat client
            var client = new ChatClient(address, port);

            // Connect the client
            Console.Write("Client connecting...");
            client.ConnectAsync();
            Console.WriteLine("Done!");

            Console.WriteLine("Press Enter to stop the client or '!' to reconnect the client...");

            // Perform text input
            for (; ; )
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Disconnect the client
                if (line == "!")
                {
                    Console.Write("Client disconnecting...");
                    client.DisconnectAsync();
                    Console.WriteLine("Done!");
                    continue;
                }

                // Send the entered text to the chat server
                client.SendTextAsync(line);
            }

            // Disconnect the client
            Console.Write("Client disconnecting...");
            client.DisconnectAndStop();
            Console.WriteLine("Done!");


            Console.WriteLine("Hello, World!");
#endif

#if PROCESS_REPLAY

            if (1 == 2)
            {


                RecordPoint[] points = new RecordPoint[2] { new RecordPoint() { FieldName = "V1", TagName = "aaV1" }, new RecordPoint() { FieldName = "V2", TagName = "aaV2" } };



                //server hv self
                var server1 = "172.168.101.120";
                var token1 = "TMbbVaiFTGVEeqdyuORfF70nI5njA2E5Fsjxe7lQTVPhy_rL0hoSjyXFa1QLgYFBaqUQgfqBgc2x6SScThPm3A==";

                //server 79 dell office
                var server2 = "192.168.1.79";
                var token2 = "ghLrzOh1-Z31VPqsF1fX59UKj3dwYFkw1udiGyLgoORvJkWfDjcpipm5QdktjfibNA64m6pK7EkOEeHfLkI1Zw==";


                var ctrl = ReplyController.Build(StoreClient_TE.InfluxDB, new DataSetConfigure()
                {
                    Server = server1,
                    Port = 8086,
                    Org = "Demo01",
                    Database = "Playback",
                    Token = token1,
                    Table = "Test1"

                }, points);


                var bbbbbbb = ctrl.TestConnect();


                ctrl.QueryCompleted += (s, e) =>
                {
                    Console.WriteLine("i am back+++++++++++++++++++");

                    foreach (var r in e.Results)
                    {
                        Console.WriteLine($"{r.Timestamp},{r.Name},{r.Value} ");
                    }
                };

                ctrl.ReplayTime = new DateTime(2025, 10, 15, 14, 29, 00);

                Console.WriteLine("turn on");
                ctrl.Enabled = true;

                Console.WriteLine("go");

                for (var i = 0; i < 100; i++)
                {
                    Task.Delay(100).GetAwaiter().GetResult();
                    ctrl.ReplayTime = ctrl.ReplayTime.AddSeconds(2);
                }

            }


            {


                RecordPoint[] points = new RecordPoint[2] { new RecordPoint() { FieldName = "TT1", TagName = "V1" }, new RecordPoint() { FieldName = "TT2", TagName = "V2" } };

                var ctrl = ReplyController.Build(StoreClient_TE.MSSQL, new DataSetConfigure()
                {
                    Server = "172.168.101.120",
                    Port = 1433,
                    Database = "XijingDB",
                    User = "sa",
                    Password = "123",
                    Table = "SensorData"

                }, points);
                var bbbbbbb = ctrl.TestConnect();

                ctrl.QueryCompleted += (s, e) =>
                {
                    Console.WriteLine("i am back+++++++++++++++++++");

                    foreach (var r in e.Results)
                    {
                        Console.WriteLine($"{r.Timestamp},{r.Name},{r.Value} ");
                    }
                };

                ctrl.ReplayTime = new DateTime(2025, 12, 15, 14, 52, 50);

                Console.WriteLine("turn on");
                ctrl.Enabled = true;

                Console.WriteLine("go");

                for (var i = 0; i < 100; i++)
                {
                    Task.Delay(100).GetAwaiter().GetResult();
                    ctrl.ReplayTime = ctrl.ReplayTime.AddSeconds(2);
                }
            }


            Console.ReadLine();


#endif

            //SQL_Maintenance();


#if SCRIBAN
            Scriban_Test();
#endif
            //string pattern = "aa{#ab}11";

            //Console.WriteLine(string.Format(pattern, DateTime.Now));

#if DXF
            PID_LOADER();
#endif







            Console.WriteLine("done");
        }

        static void SQL_Maintenance() {


            FTOptixNetPlugin.DatabaseMaintenance.MSSQL sqlCli = new FTOptixNetPlugin.DatabaseMaintenance.MSSQL("172.168.101.120", 1433, "sa", "123", 30);

            var dbName = "DB_Mgt_Test";
            
            var bExit = false;
            var backup_folder = @"C:\database\mssql\backup";
            while (true)
            {
                Console.WriteLine("enter code");
                Console.WriteLine(" 1:DB Exists");
                Console.WriteLine(" 2:Detach DB");
                Console.WriteLine(" 3:List DB Files");
                Console.WriteLine(" 4:Attach DB Files");
                Console.WriteLine(" 5:copy DB Files");
                Console.WriteLine(" 6:List Folder Files");
                Console.WriteLine(" 7:List Volumn Status");
                Console.WriteLine(" 8:Backup");
                var c = Console.ReadLine();

                switch (c)
                {
                    case "1":
                        var r = sqlCli.IsExistsDatabase(dbName);
                        Console.WriteLine($"database {dbName} is exists:{r}");
                        break;
                    case "3":
                        var files = sqlCli.GetDatabaseFilePath(dbName);
                        foreach (var kv in files)
                        {
                            Console.WriteLine($"{kv.Key}  {kv.Value}");
                        }

                        break;
                    case "2":
                        var rr = sqlCli.DetachDatabase(dbName);
                        Console.WriteLine($"detach database {dbName} is exists:{rr}");
                        break;
                    case "4":
                        var mdf = @"C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\DB_Mgt_Test.mdf";
                        var ldf = @"C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\DB_Mgt_Test_log.ldf";
                        var new_dbname = dbName;
                        rr = sqlCli.AttachDatabase(new_dbname, mdf, ldf);

                        Console.WriteLine($"attach database {new_dbname} is exists:{rr}");
                        break;
                    case "5":
                        mdf = @"C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\DB_Mgt_Test.mdf";
                        ldf = @"C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\DB_Mgt_Test_log.ldf";
                        string n_mdf, n_ldf;

                        (n_mdf, n_ldf) = sqlCli.RenameDatabaseFile(backup_folder, "backup_aabbcc");

                        if (sqlCli.CopyDatabaseFile(mdf, n_mdf))
                        {
                            if(sqlCli.CopyDatabaseFile(ldf, n_ldf))
                            {
                                Console.WriteLine($"copy database file is ok");
                            }

                        }



                        break;
                    case "6":

                        var fff = sqlCli.ListFolderFiles(backup_folder);
                        foreach(var filepath in fff)
                        {
                            Console.WriteLine(filepath);
                        }

                        break;
                    case "7":

                        var vols = sqlCli.GetVolumnStatus();
                        foreach (var vol in vols)
                        {
                            Console.WriteLine($"{vol.Name} : total:{vol.TotalSpace}GB Used:{vol.UsedSpace}GB Free:{vol.FreeSpace}GB");
                        }

                        break;

                    case "8":
                        
                        if (sqlCli.IsExistsDatabase(dbName) == 1)
                        {

                            var dbfiles = sqlCli.GetDatabaseFilePath(dbName);
                            dbfiles.TryGetValue(dbName, out var old_mdf);
                            dbfiles.TryGetValue(dbName + "_log", out var old_ldf);

                            



                            if (sqlCli.DetachDatabase(dbName))
                            {
                                (var new_mdf, var new_ldf) = sqlCli.RenameDatabaseFile(backup_folder, "backup_new1");


                                sqlCli.CopyDatabaseFile(old_mdf, new_mdf);
                                sqlCli.CopyDatabaseFile(old_ldf, new_ldf);



                                sqlCli.AttachDatabase(dbName, old_mdf, old_ldf);
                                if (sqlCli.IsExistsDatabase(dbName) == 1)
                                {
                                    Console.WriteLine("OK");
                                }
                            }
                        }


                        break;
                    case "q":
                        bExit = true;
                        break;
                }
                if (bExit)
                {
                    break;
                }

            }












            //var r = sqlCli.IsExistsDatabase(dbName);
            //Console.WriteLine($"database {dbName} is exists:{r}");


            //var files = sqlCli.GetDatabaseFilePath(dbName);
            //foreach (var kv in files)
            //{
            //    Console.WriteLine($"{kv.Key}  {kv.Value}");
            //}

            //if (r == 1)
            //{
            //    var rr = sqlCli.DetachDatabase(dbName);
            //    Console.WriteLine($"detach database {dbName} is exists:{rr}");

            //    System.Threading.Thread.Sleep(5000);


            //    r = sqlCli.IsExistsDatabase(dbName);
            //    Console.WriteLine($"database {dbName} is exists:{r}");


            //    Console.WriteLine("detach ok,enter key to continue");
            //    Console.ReadLine();

            //    var mdf = files["DB_Mgt_Test"];
            //    var ldf = files["DB_Mgt_Test_log"];
            //    var new_dbname = dbName + "_22";
            //    rr = sqlCli.AttachDatabase(new_dbname, mdf, ldf);

            //    Console.WriteLine($"attach database {new_dbname} is exists:{rr}");


            //    System.Threading.Thread.Sleep(5000);


            //    r = sqlCli.IsExistsDatabase(new_dbname);
            //    Console.WriteLine($"database {new_dbname} is exists:{r}");
            //}

        }


#if SCRIBAN
        static void Scriban_Test()
        {
            string FormatErrors(IEnumerable<LogMessage> messages)
            {
                return "Template parsing failed:\n" + string.Join("\n", messages);
            }


            var tpl = @"
{{ for item in vals }}
a = {{ item.a }};
b = {{ item.b }}

{{ end }}
";

            var obj = new {
                vals = new object[]
                {
                    new { a=1,b=11 },
                    new { a=2,b=22 },
                },
                
                
            };

            var _template = Scriban.Template.Parse(tpl);
            if (_template.HasErrors)
            {
                // 在构造时就抛出异常，实现快速失败
                throw new InvalidOperationException(FormatErrors(_template.Messages));
            }

            var context = new TemplateContext
            {
                // 关键配置: 支持 include 子模板
                //TemplateLoader = _templateLoader,
                // 关键配置: 支持 C# 命名风格 (PascalCase)
                MemberRenamer = member => member.Name
            };

            var scriptObject = new ScriptObject();
            if (obj != null)
            {
                scriptObject.Import(obj);
            }
            context.PushGlobal(scriptObject);

            // 使用 Task.Run 来将同步的 Render 方法包装成异步的
            var txt = _template.RenderAsync(context).GetAwaiter().GetResult();
            Console.WriteLine(tpl);
            Console.WriteLine(txt);
        }

#endif


#if DXF
        static void PID_LOADER()
        {
            var filepath = @"D:\Work\Projects\PID Loader\2024-010Pb20240910.dxf";
            var doc = Document.LoadDxf(filepath,0 ,0,0,0, true,out var err);
            Console.Write(err);
            Console.WriteLine();
            if (doc != null)
            {
                //var c = doc.Dxf.DrawingVariables.CustomValues();
                Console.WriteLine("Blocks");
                foreach (var b in doc.Dxf.Blocks)
                {
                    Console.WriteLine($"Block Name:{b.Name}");
                }



                var texts = doc.GetTexts();

                Console.WriteLine("TEXT");

                foreach(var v in texts)
                {
                    Console.WriteLine($"- text:{v.Value} , rotation:{v.Rotation:f3} , position:{v.Position.X} {v.Position.Y}");
                }

                var polylines = doc.GetPolylines();
                Console.WriteLine("POLYLINE");
                foreach (var v in polylines)
                {
                    Console.WriteLine($"- color:{v.Color.R} {v.Color.G} {v.Color.B} {v.Color.A} , thiness:{v.Thickness}");
                    foreach(var p in v.Vertexes)
                    {
                        Console.WriteLine($"  point:{p.X} {p.Y} {p.Z}");
                    }
                }


                var blocks = doc.GetBlockReferences();
                Console.WriteLine("BLOCK INSERT");
                foreach (var v in blocks)
                {
                    Console.WriteLine($"- name:{v.BlockName} , rotation:{v.Rotation:f3} , position:{v.Position.X} {v.Position.Y} , scale:{v.Scale.X} {v.Scale.Y}");
                }

                //foreach(var b in doc.Dxf.Blocks)
                //{
                //    Console.WriteLine("block name:{0}", b.Name);
                //}
            }
            
        }
#endif
    }



}
