using FFmpeg.AutoGen;
using FTOptixNetPlugin.NetServer.Http;
using FTOptixNetPlugin.NetServer.Mvc;
using RTSPPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSPPlayer.WebApi
{
    public class T1Controller:Controller
    {
        readonly MainWindowViewModel _vm;

         int _count = 0;

        public T1Controller( MainWindowViewModel vm) { 
        
            _vm = vm;
        }



        [Route("GET")]
        public IResult Test()
        {

            _vm.WindowTitle = $"I get a message {_count}";


            return new TextResult("hello");

        }


    }
}
