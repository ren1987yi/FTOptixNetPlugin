using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService.XSSheet.Model
{
    public class Cell
    {
        public Cell()
        {

        }

        public Cell(string txt)
        {
            text = txt;
        }

        public string text { get; set; }
    }

}
