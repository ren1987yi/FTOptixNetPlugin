using Process.PIDLoader.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Process.PIDLoader
{
    public class ProjectDocument
    {
        public string CADFilePath { get; set; }

        public float XMin { get; set; }
        public float YMin { get; set; }
        public float XMax { get; set; }
        public float YMax { get; set; }


        public bool IgnoreNoBindBlock { get; set; } = true;

        public bool DrawLine { get; set; } = true;
        public bool DrawText { get; set; } = true;
        public bool DrawBlock { get; set; } = true;

        public List<Block> Blocks { get; set; } = new List<Block>();

        public List<IElement> Elements { get; set; } = new List<IElement>();


        public BBox BBox { get; private set; }
        public ProjectDocument()
        {
            
        }

        private void init()
        {
            BBox = new BBox(new System.Numerics.Vector2(XMin, YMin), new System.Numerics.Vector2(XMax, YMax));
        }




        public static ProjectDocument LoadFile(string filepath)
        {
            try
            {

                var t = File.ReadAllText(filepath);
                var setting = new Newtonsoft.Json.JsonSerializerSettings();
                setting.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectDocument>(t, setting);
                obj.init();
                return obj;
            }
            catch
            {
                return null;
            }
        }

    
    
        
    
    }
}
