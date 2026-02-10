using ACadSharp;
using ACadSharp.IO;
using CADExtract.Model;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;

namespace CADExtract
{
    public class Extractor
    {


        string _filepath = string.Empty;
        public Extractor(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException();
            }

            _filepath = filepath;

            

        }


        public Model.CAData Read(Viewport view)
        {
            var ext = Path.GetExtension(_filepath).ToLower();
            switch (ext)
            {
                case ".dwg":
                    return extratDwg(_filepath,view);
                    break;
                case ".dxf":
                    return extratDxf(_filepath,view);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cad file format is not support");
                    break;
            }

        }


        private Model.CAData extratDwg(string filepath,Viewport view)
        {
            CadDocument doc;

            
            using (DwgReader reader = new DwgReader(filepath))
            {
                doc = reader.Read();

            }

            var cad = new CAData();
            cad.FileInfo.Name = Path.GetFileName(_filepath);
            cad.FileInfo.Viewport = view;
            cad.FileInfo.Data = File.ReadAllBytes(_filepath);

            foreach (var block in doc.BlockRecords)
            {
                cad.Blocks.Add(new Block(block));
            }
                
            foreach(var el in doc.Entities)
            {
                switch (el.ObjectType)
                {
                    case ObjectType.INSERT:
                        cad.Entities.Add(new InsertEntity(el as ACadSharp.Entities.Insert));
                        break;
                    case ObjectType.TEXT:
                        cad.Entities.Add(new TextEntity(el as ACadSharp.Entities.TextEntity));
                        break;
                    case ObjectType.MTEXT:
                        cad.Entities.Add(new TextEntity(el as ACadSharp.Entities.MText));
                        break;
                    case ObjectType.LINE:
                        cad.Entities.Add(new PolylineEntity(el as ACadSharp.Entities.Line));
                        break;
                    case ObjectType.MLINE:
                        cad.Entities.Add(new PolylineEntity(el as ACadSharp.Entities.MLine));
                        break;
                    case ObjectType.LWPOLYLINE:
                        cad.Entities.Add(new PolylineEntity(el as ACadSharp.Entities.LwPolyline));
                        break;
                    case ObjectType.POLYLINE_2D:
                        cad.Entities.Add(new PolylineEntity(el as ACadSharp.Entities.Polyline2D));
                        break;
                    case ObjectType.POLYLINE_3D:
                        cad.Entities.Add(new PolylineEntity(el as ACadSharp.Entities.Polyline3D));
                        break;

                    default:
                        continue;
                        break;

                }
            }


            return cad;
            
            
        }


        private Model.CAData extratDxf(string filepath, Viewport view)
        {
            var cad = new CAData();
            cad.FileInfo.Name = Path.GetFileName(_filepath);
            cad.FileInfo.Viewport = view;
            return null;
        }

    }
}
