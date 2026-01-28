using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetProtainerApi.Model
{
    public class Container
    {
        public string Id { get; set; }
        public List<string> Names { get; set; }
        public string Image { get; set; }
        public string ImageID { get; set; }
        public long Created { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
    }
}
