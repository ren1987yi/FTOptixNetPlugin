using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessReply
{
    /// <summary>
    /// 记录点
    /// </summary>
    public class RecordPoint
    {
        public string FieldName { get; set; }
        public string TagName { get; set; }

        
        public object Value { get=>_value;  }



        private object _value = null;

        public void SetValue(object value)
        {
            _value = value;
        }


        
    }
}
