using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace HIKPlayer.Controllers
{
    [RoutePrefix("console")]
    public class ConsoleController:ApiController
    {

      


        [HttpGet]
        [Route("show")]
        public IHttpActionResult Show()
        {
            App.Command.Show();

            return Ok();
        }


        [HttpGet]
        [Route("layout1")]
        public IHttpActionResult Layout1()
        {
            App.Command.Layout1();

            return Ok();
        }

        [HttpGet]
        [Route("layoutall")]
        public IHttpActionResult LayoutAll()
        {
            App.Command.LayoutAll();

            return Ok();
        }


        [HttpPost]
        [Route("loc")]
        public async Task<IHttpActionResult> Loc()
        {
            //App.Command.Show();

            var txt = await Request.Content.ReadAsStringAsync();



            var data =  JsonConvert.DeserializeObject<Translate>(txt);
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

            App.Command.Location(ds);

            return Ok();
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
