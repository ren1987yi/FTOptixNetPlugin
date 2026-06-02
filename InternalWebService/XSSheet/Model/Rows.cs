using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService.XSSheet.Model
{
    public class Rows : Dictionary<string, Row>
    {
        public int len { get; set; } = 100;
    }
}
