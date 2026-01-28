using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessReply
{
    /// <summary>
    /// 数据集的配置
    /// </summary>
    public class DataSetConfigure
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Table { get; set; }
        public string Org { get; set; }
    }
}
