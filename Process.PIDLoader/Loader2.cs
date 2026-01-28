using FTOptix.Core;
using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptixNetPlugin.Extensions;
using Process.PIDLoader.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UAManagedCore;

namespace Process.PIDLoader
{
    public class Loader2
    {
        ProjectDocument _prj;
        public Loader2(ProjectDocument prj)
        {
            _prj = prj;
        }




        private System.Numerics.Vector3 CoordinateTransformation(System.Numerics.Vector3 point)
        {
            var npoint = System.Numerics.Vector3.Zero;

            npoint.X = point.X - _prj.BBox.XMin;
            npoint.Y = _prj.BBox.YMax - point.Y;

            return npoint;
        }

        public void Draw(IUANode parent,Dictionary<string,IUANode> blockMapper)
        {
           


            foreach(var el in _prj.Elements)
            {
                if(el is Text)
                {
                    if (!_prj.DrawText)
                    {
                        continue;
                    }
                    DrawText(parent, (Text)el);

                }else if (el is Process.PIDLoader.Elements.Polyline)
                {
                    if (!_prj.DrawLine)
                    {
                        continue;
                    }
                    DrawLine(parent, (Process.PIDLoader.Elements.Polyline)el);

                }
                else if(el is BlockReference)
                {
                    if (!_prj.DrawBlock)
                    {
                        continue;
                    }

                    DrawBlock(parent,(BlockReference)el , blockMapper);
                }
            }


        }

        public void DrawText(IUANode parent, Text item)
        {
            var npos = CoordinateTransformation(item.Position);
            var pos = new Vector2(npos.X, npos.Y);
            var lb = Painter.AddLabel(parent, pos, item.Value);
            
        }

        public void DrawLine(IUANode parent, Process.PIDLoader.Elements.Polyline item)
        {
            var nps = new List<Vector2>();
            foreach(var p in item.Vertexes)
            {
                var np = CoordinateTransformation(p);
                nps.Add(new Vector2(np.X,np.Y));
            }
            //item.Color
            Color c = new Color(item.Color.A, item.Color.R, item.Color.G, item.Color.B);


            Painter.AddPolyline(parent, nps,c, Colors.Transparent);

        }


        public void DrawBlock(IUANode parent,BlockReference block, Dictionary<string,IUANode> mapper)
        {

            var blockRecord = _prj.Blocks.Where(b => b.Name == block.BlockName).FirstOrDefault();
            if(blockRecord == null)
            {
                return;
            }

            var uitypeName = blockRecord.BindName;

            if (mapper.TryGetValue(uitypeName, out var ui))
            {
                var np = CoordinateTransformation(block.Position);

                var node = Painter.AddUIObject(parent, ui, new Vector2(np.X, np.Y));
                node.Rotation = block.Rotation;
                foreach(var attr in block.Attributes)
                {
                    var val = attr.Value;
                    var tag = attr.Tag;

                    var _attr = blockRecord.Attributes.Where(a => a.Tag == tag).FirstOrDefault();
                    if(_attr != null)
                    {
                        var varname = _attr.Value;

                        node.SetVariableValue<string>(varname, val);
                    }

                }

            }
        }

    }
}
