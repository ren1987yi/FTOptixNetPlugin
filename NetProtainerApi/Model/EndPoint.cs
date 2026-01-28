using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetProtainerApi.Model
{
    internal class EndPoint
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string ContainerEngine { get; set; }
        public int GroupId { get; set; }

        public int Status { get; set; }

      
    }
}
