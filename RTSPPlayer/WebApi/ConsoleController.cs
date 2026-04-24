using FTOptixNetPlugin.NetServer.Http;
using FTOptixNetPlugin.NetServer.Mvc;
using RTSPPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RTSPPlayer.WebApi
{
    public class ConsoleController:Controller
    {

        readonly MainWindowViewModel _vm;

      

        public ConsoleController(MainWindowViewModel vm)
        {

            _vm = vm;
        }

        [Route("GET")]
        public IResult Shutdown()
        {
            _vm.ShutdownCommand.Execute(null);
            return new Ok();
        }


        [Route("GET")]
        public IResult Show()
        {
            //_vm.Window.ShowHandle();
            
            _vm.WindowVisibility = System.Windows.Visibility.Visible;
            return new Ok();
        }


        [Route("GET")]
        public IResult Hide()
        {

            // 异步非阻塞调用（推荐常规使用）
          
            //_vm.Window.HideHandle();
            _vm.WindowVisibility = System.Windows.Visibility.Collapsed;

            return new Ok();
        }

        [Route("GET")]
        public IResult Play()
        {
            _vm.OpenCommand.Execute(null); return new Ok();
            return new Ok();
        }



        [Route("GET")]
        public IResult Layout11()
        {
            
            _vm.Layout1_1Command.Execute(null); return new Ok();
            return new Ok();
        }

        [Route("GET")]
        public IResult Layout22()
        {
            _vm.Layout2_2Command.Execute(null); return new Ok();
            return new Ok();
        }


        [Route("POST")]
        public IResult Move()
        {

            var txt = Request.Body;

            var data = JsonSerializer.Deserialize<Location>(txt);
            if(data == null)
            {
                data = new Location() { X = 0, Y = 0 };
            }
            _vm.WindowLeft = data.X;
            _vm.WindowTop = data.Y;


            return new Ok();
        }


        [Route("POST")]
        public IResult Size()
        {

            var txt = Request.Body;

            var data = JsonSerializer.Deserialize<Location>(txt);
            if (data == null)
            {
                data = new Location() { X = 0, Y = 0 };
            }
            _vm.WindowWidth = data.X;
            _vm.WindowHeight = data.Y;


            return new Ok();
        }

        [Route("POST")]
        public IResult Loc()
        {

            var txt = Request.Body;

            var data = JsonSerializer.Deserialize<Translate>(txt);
            if (data == null)
            {
                data = new Translate()
                {
                    Size = new Location() { X = 800, Y = 600 },
                    Pos = new Location() { X = 0, Y = 0 }
                };
            }

            var ds = new int[4];
            ds[0] = data.Pos.X; ds[1] = data.Pos.Y; ds[2] = data.Size.X; ds[3] = data.Size.Y;
            _vm.TranslateCommand.Execute(ds);

            return new Ok();
        }



        class Location
        {
            public int X { get; set; }    
            public int Y { get; set; }
        }

        class Translate
        {
            public Location Pos { get; set; }
            public Location Size { get; set; }  
        }
        
    }
}
