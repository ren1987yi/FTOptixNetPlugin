using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptixNetPlugin.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UAManagedCore;

namespace Process.PIDLoader
{
    public class Loader
    {
        Document _doc;

        public Loader(Document doc)
        {
            _doc = doc;
        }

        public static Loader BuildFromDxf(string filepath, LoadConfiguration config)
        {
            var doc = Document.LoadDxf(filepath,config.PageSize.XMin,config.PageSize.YMin,config.PageSize.XMax,config.PageSize.YMax,true,out var output);

            return new Loader(doc);
        }

        public static Loader BuildFromDwg(string filepath, LoadConfiguration config)
        {
            var doc = Document.LoadDwg(filepath, config.PageSize.XMin, config.PageSize.YMin, config.PageSize.XMax, config.PageSize.YMax, true, out var output);

            return new Loader(doc);
        }



        public void LoadTexts(IUANode parent)
        {
            var texts = _doc.GetTexts();

            foreach(var txt in texts)
            {
                var pos = new Vector2(txt.Position.X, txt.Position.Y);
                var lb = Painter.AddLabel(parent, pos, txt.Value);
            }

        }


        public void LoadPolyline(IUANode parent)
        {
            var lines = _doc.GetPolylines();

            foreach (var line in lines)
            {
                var ps = line.Vertexes.Select(p => new Vector2(p.X, p.Y));
                var lb = Painter.AddPolyline(parent, ps, Colors.Black,Colors.Transparent);
                
            }




        }


        public void LoadBlockReferences(IUANode parent,Dictionary<string,IUANode> blockMapper)
        {
            var blocks = _doc.GetBlockReferences();

            foreach(var b in blocks)
            {
                if(blockMapper.TryGetValue(b.BlockName,out var uiNode))
                {
                    var pos = new Vector2(b.Position.X, b.Position.Y);
                    Painter.AddUIObject(parent, uiNode, pos);

                }
            }


        }



    }
}
