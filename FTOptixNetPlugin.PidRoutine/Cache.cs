using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.PidRoutine
{

    public  interface ICache
    {
        public void Refresh();
    }


    public interface ICache<T> : ICache where T:class,new()
    {
        public T Get(string key);
        public void Remove(string key);
    
    }






}
