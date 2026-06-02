using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService.XSSheet.Model
{
    public class sheet
    {
        public string name { get; set; }
        public string freeze { get; set; } = "A1";
        public object[] styles { get; set; } = new object[] { };
        public object[] merges { get; set; } = new object[] { };


        public Rows rows { get; set; } = new Rows();
        public Columns cols { get; set; } = new Columns();

        public object[] validations { get; set; } = new object[] { };


        public Autofilter autofilter { get; set; } = new Autofilter();
    }

}
