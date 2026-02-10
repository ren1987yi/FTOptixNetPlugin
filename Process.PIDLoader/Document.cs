using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.IO;
using netDxf;
using netDxf.Header;
using Process.PIDLoader.Elements;


namespace Process.PIDLoader
{
    public class Document
    {


        static string OBJECT_NAME = typeof(Document).FullName;


        #region private properties
        private DxfDocument _dxf;
        private CadDocument _dwg;
        #endregion

        #region public properties

        public DxfDocument Dxf => _dxf;
        public CadDocument Dwg => _dwg;

        /// <summary>
        /// dxf content bounding box
        /// </summary>
        public BBox BBox { get; private set; }

        public DocumentType_TE DocumentType { get; private set; } = DocumentType_TE.UnKown;


        #endregion


        #region constructor

        private Document(CadDocument doc)
        {
            _dwg = doc;
            DocumentType = DocumentType_TE.DWG;
            //System.Numerics.Vector3 _min = new System.Numerics.Vector3( 
            //    (float)_dxf.DrawingVariables.ExtMin.X
            //    , (float)_dxf.DrawingVariables.ExtMin.Y
            //    , (float)_dxf.DrawingVariables.ExtMin.Z);

            //System.Numerics.Vector3 _max = new System.Numerics.Vector3(
            //    (float)_dxf.DrawingVariables.ExtMax.X
            //    , (float)_dxf.DrawingVariables.ExtMax.Y
            //    , (float)_dxf.DrawingVariables.ExtMax.Z);


            BBox = new BBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero);
        }



        private Document(DxfDocument doc)
        {
            _dxf = doc;
            DocumentType = DocumentType_TE.DXF;
            //System.Numerics.Vector3 _min = new System.Numerics.Vector3( 
            //    (float)_dxf.DrawingVariables.ExtMin.X
            //    , (float)_dxf.DrawingVariables.ExtMin.Y
            //    , (float)_dxf.DrawingVariables.ExtMin.Z);

            //System.Numerics.Vector3 _max = new System.Numerics.Vector3(
            //    (float)_dxf.DrawingVariables.ExtMax.X
            //    , (float)_dxf.DrawingVariables.ExtMax.Y
            //    , (float)_dxf.DrawingVariables.ExtMax.Z);


            BBox = new BBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero);
        }



        private Document(DxfDocument doc, System.Numerics.Vector3 min, System.Numerics.Vector3 max)
        {
            _dxf = doc;
            DocumentType = DocumentType_TE.DXF;
            //System.Numerics.Vector3 _min = new System.Numerics.Vector3( 
            //    (float)_dxf.DrawingVariables.ExtMin.X
            //    , (float)_dxf.DrawingVariables.ExtMin.Y
            //    , (float)_dxf.DrawingVariables.ExtMin.Z);

            //System.Numerics.Vector3 _max = new System.Numerics.Vector3(
            //    (float)_dxf.DrawingVariables.ExtMax.X
            //    , (float)_dxf.DrawingVariables.ExtMax.Y
            //    , (float)_dxf.DrawingVariables.ExtMax.Z);


            BBox = new BBox(min, max);
        }


        private Document(CadDocument doc, System.Numerics.Vector3 min, System.Numerics.Vector3 max)
        {
            _dwg = doc;
            DocumentType = DocumentType_TE.DWG;
            //System.Numerics.Vector3 _min = new System.Numerics.Vector3( 
            //    (float)_dxf.DrawingVariables.ExtMin.X
            //    , (float)_dxf.DrawingVariables.ExtMin.Y
            //    , (float)_dxf.DrawingVariables.ExtMin.Z);

            //System.Numerics.Vector3 _max = new System.Numerics.Vector3(
            //    (float)_dxf.DrawingVariables.ExtMax.X
            //    , (float)_dxf.DrawingVariables.ExtMax.Y
            //    , (float)_dxf.DrawingVariables.ExtMax.Z);


            BBox = new BBox(min, max);
        }
        #endregion


        #region private methods
        private System.Numerics.Vector3 CoordinateTransformation(System.Numerics.Vector3 point)
        {
            var npoint = System.Numerics.Vector3.Zero;

            npoint.X = point.X - BBox.XMin;
            npoint.Y = BBox.YMax - point.Y;

            return npoint;
        }
        #endregion


        #region public methods


        public static Document LoadDxf(string file, bool debug, out string output)
        {
            output = string.Empty;
            FileInfo fileInfo = new FileInfo(file);
            if (!fileInfo.Exists)
            {
                if (debug)
                {

                    output += $"THE FILE {file} DOES NOT EXIST \n";
                }

                return null;
            }

            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out bool isBinary);

            if (dxfVersion == DxfVersion.Unknown)
            {
                if (debug)
                {
                    output += $"THE FILE {file} DOES NOT EXIST \n";
                }
                return null;
            }

            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                if (debug)
                {
                    output += $"THE FILE {file} IS NOT A SUPPORTED DXF\n";
                    output += $"FILE VERSION: {dxfVersion}\n";
                }
                return null;
            }

            DxfDocument dxf = DxfDocument.Load(file);

            if (dxf == null)
            {
                if (debug)
                {
                    output += string.Format("ERROR LOADING {0}\n", file);
                }

                return null;
            }


            return new Document(dxf);


        }

        public static Document LoadDxf(string file, float xmin, float ymin, float xmax, float ymax, bool debug, out string output)
        {
            output = string.Empty;
            FileInfo fileInfo = new FileInfo(file);
            if (!fileInfo.Exists)
            {
                if (debug)
                {

                    output += $"THE FILE {file} DOES NOT EXIST \n";
                }
                
                return null;
            }

            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out bool isBinary);

            if (dxfVersion == DxfVersion.Unknown)
            {
                if (debug)
                {
                    output += $"THE FILE {file} DOES NOT EXIST \n";
                }
                return null;
            }

            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                if (debug)
                {
                    output += $"THE FILE {file} IS NOT A SUPPORTED DXF\n";
                    output += $"FILE VERSION: {dxfVersion}\n";
                }
                return null;
            }

            DxfDocument dxf = DxfDocument.Load(file);
            
            if (dxf == null)
            {
                if (debug)
                {
                    output += string.Format("ERROR LOADING {0}\n", file);
                }

                return null;
            }


            return new Document(dxf, new System.Numerics.Vector3(xmin, ymin, 0), new System.Numerics.Vector3(xmax, ymax, 0));


        }




        public static Document LoadDwg(string file, bool debug, out string output)
        {
            CadDocument doc;
            output = string.Empty;
            try
            {
                using (DwgReader reader = new DwgReader(file))
                {
                    doc = reader.Read();
                    
                }
                var b = doc.BlockRecords.First();
                var e = b.Entities.First();
                
   
                
                return new Document(doc);

            }
            catch (Exception ex)
            {

                if (debug)
                {
                    output = ex.Message;
                }
                return null;
            }

        }

        public static Document LoadDwg(string file, float xmin, float ymin, float xmax, float ymax, bool debug, out string output)
        {
            CadDocument doc;
            output = string.Empty;
            try
            {
                using (DwgReader reader = new DwgReader(file))
                {
                    doc = reader.Read();
                }
                return new Document(doc, new System.Numerics.Vector3(xmin, ymin, 0), new System.Numerics.Vector3(xmax, ymax, 0));
                //return new Document(doc);

            }
            catch (Exception ex)
            {

                if (debug)
                {
                    output = ex.Message;
                }
                return null;
            }




        }


        public void SetBBox(System.Numerics.Vector3 pmin, System.Numerics.Vector3 pmax)
        {
            this.BBox = new BBox(pmin, pmax);
        }


        public IEnumerable<Text> GetTexts(bool transformation = true)
        {
            switch (this.DocumentType)
            {
                case DocumentType_TE.DXF:
                    return GetDxfTexts(transformation);
                case DocumentType_TE.DWG:
                    return GetDwgTexts(transformation);
                default:
                    throw new ArgumentException();
            }
        }
        public IEnumerable<Polyline> GetPolylines(bool transformation = true)
        {
            switch (this.DocumentType)
            {
                case DocumentType_TE.DXF:
                    return GetDxfPolylines(transformation);
                case DocumentType_TE.DWG:
                    return GetDwgPolylines(transformation);
                default:
                    throw new ArgumentException();
            }
        }


        public IEnumerable<BlockReference> GetBlockReferences(bool transformation = true)
        {
            switch (this.DocumentType)
            {
                case DocumentType_TE.DXF:
                    return GetDxfBlockReferences(transformation);
                case DocumentType_TE.DWG:
                    return GetDwgBlockReferences(transformation);
                default:
                    throw new ArgumentException();
            }
        }


        public IEnumerable<Block> GetBlocks()
        {
            switch (this.DocumentType)
            {
                case DocumentType_TE.DXF:
                    return GetDxfBlocks();
                case DocumentType_TE.DWG:
                    return GetDwgBlocks();
                default:
                    throw new ArgumentException();
            }
        }





        private IEnumerable<Text> GetDxfTexts(bool transformation = true)
        {
            var ts = new List<Text>();
            foreach (var t in _dxf.Entities.Texts)
            {
                var pos = new System.Numerics.Vector3(
                    (float)t.Position.X,
                    (float)t.Position.Y,
                    (float)t.Position.Z

                    );
                if (transformation)
                {

                    pos = this.CoordinateTransformation(pos);
                }
                ts.Add(new Text(t.Value, pos, t.Rotation));

            }

            foreach (var t in _dxf.Entities.MTexts)
            {
                var pos = new System.Numerics.Vector3(
                    (float)t.Position.X,
                    (float)t.Position.Y,
                    (float)t.Position.Z

                    );
                if (transformation)
                {

                    pos = this.CoordinateTransformation(pos);
                }
                ts.Add(new Text(t.Value, pos, t.Rotation));

            }

            return ts;
        }
        private IEnumerable<Text> GetDwgTexts(bool transformation = true)
        {
            var ts = new List<Text>();
            foreach (var t in _dwg.Entities.OfType<TextEntity>())
            {
                var pos = new System.Numerics.Vector3(
                    (float)t.InsertPoint.X,
                    (float)t.InsertPoint.Y,
                    (float)t.InsertPoint.Z

                    );
                if (transformation)
                {

                    pos = this.CoordinateTransformation(pos);
                }
                ts.Add(new Text(t.Value, pos, t.Rotation));

            }

            foreach (var t in _dwg.Entities.OfType<MText>())
            {
                var pos = new System.Numerics.Vector3(
                    (float)t.InsertPoint.X,
                    (float)t.InsertPoint.Y,
                    (float)t.InsertPoint.Z

                    );
                if (transformation)
                {

                    pos = this.CoordinateTransformation(pos);
                }
                ts.Add(new Text(t.Value, pos, t.Rotation));

            }

            return ts;
        }


        private IEnumerable<Polyline> GetDxfPolylines(bool transformation = true)
        {
            var ls = new List<Polyline>();

            foreach (var l in _dxf.Entities.Lines)
            {
                var c = new RGBA(l.Color.R, l.Color.G, l.Color.B, 255);
                var ps = new List<System.Numerics.Vector3>();

                if (transformation)
                {

                    ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                        (float)l.StartPoint.X,
                        (float)l.StartPoint.Y,
                        (float)l.StartPoint.Z
                        )));

                    ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                        (float)l.EndPoint.X,
                        (float)l.EndPoint.Y,
                        (float)l.EndPoint.Z
                        )));
                }
                else
                {
                    ps.Add(new System.Numerics.Vector3(
                        (float)l.StartPoint.X,
                        (float)l.StartPoint.Y,
                        (float)l.StartPoint.Z
                    ));

                    ps.Add(new System.Numerics.Vector3(
                        (float)l.EndPoint.X,
                        (float)l.EndPoint.Y,
                        (float)l.EndPoint.Z
                    ));
                }


                ls.Add(new Polyline(ps, l.Thickness, c));

            }


            foreach (var l in _dxf.Entities.MLines)
            {
                var c = new RGBA(l.Color.R, l.Color.G, l.Color.B, 255);
                var ps = new List<System.Numerics.Vector3>();

                foreach (var v in l.Vertexes)
                {
                    if (transformation)
                    {

                        ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                            (float)v.Position.X,
                            (float)v.Position.Y,
                            (float)0.0f
                         )));
                    }
                    else
                    {
                        ps.Add(new System.Numerics.Vector3(
                            (float)v.Position.X,
                            (float)v.Position.Y,
                            (float)0.0f
                        ));
                    }
                }

                ls.Add(new Polyline(ps, 1, c));
            }

            foreach (var pl in _dxf.Entities.Polylines2D)
            {
                var c = new RGBA(pl.Color.R, pl.Color.G, pl.Color.B, 255);
                var ps = new List<System.Numerics.Vector3>();
                foreach (var v in pl.Vertexes)
                {

                    if (transformation)
                    {
                        ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                            (float)v.Position.X,
                            (float)v.Position.Y,
                            (float)0.0f
                         )));
                    }
                    else
                    {
                        ps.Add(new System.Numerics.Vector3(
                            (float)v.Position.X,
                            (float)v.Position.Y,
                            (float)0.0f
                        ));
                    }


                }
                ls.Add(new Polyline(ps, pl.Thickness, c));
            }


            foreach (var pl in _dxf.Entities.Polylines3D)
            {
                var c = new RGBA(pl.Color.R, pl.Color.G, pl.Color.B, 255);
                var ps = new List<System.Numerics.Vector3>();
                foreach (var v in pl.Vertexes)
                {
                    if (transformation)
                    {
                        ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                            (float)v.X,
                            (float)v.Y,
                            (float)0.0f
                         )));
                    }
                    else
                    {
                        ps.Add(new System.Numerics.Vector3(
                            (float)v.X,
                            (float)v.Y,
                            (float)0.0f
                        ));
                    }

                }
                ls.Add(new Polyline(ps, 1, c));
            }


            return ls;
        }

        private IEnumerable<Polyline> GetDwgPolylines(bool transformation = true)
        {
            var ls = new List<Polyline>();

            foreach (var l in _dwg.Entities.OfType<Line>())
            {
                var c = new RGBA(l.Color.R, l.Color.G, l.Color.B, 255);
                var ps = new List<System.Numerics.Vector3>();

                if (transformation)
                {

                    ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                        (float)l.StartPoint.X,
                        (float)l.StartPoint.Y,
                        (float)l.StartPoint.Z
                        )));

                    ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                        (float)l.EndPoint.X,
                        (float)l.EndPoint.Y,
                        (float)l.EndPoint.Z
                        )));
                }
                else
                {
                    ps.Add(new System.Numerics.Vector3(
                        (float)l.StartPoint.X,
                        (float)l.StartPoint.Y,
                        (float)l.StartPoint.Z
                    ));

                    ps.Add(new System.Numerics.Vector3(
                        (float)l.EndPoint.X,
                        (float)l.EndPoint.Y,
                        (float)l.EndPoint.Z
                    ));
                }


                ls.Add(new Polyline(ps, l.Thickness, c));

            }


            foreach (var l in _dwg.Entities.OfType<MLine>())
            {
                var c = new RGBA(l.Color.R, l.Color.G, l.Color.B, 255);
                var ps = new List<System.Numerics.Vector3>();

                foreach (var v in l.Vertices)
                {
                    if (transformation)
                    {

                        ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                            (float)v.Position.X,
                            (float)v.Position.Y,
                            (float)0.0f
                         )));
                    }
                    else
                    {
                        ps.Add(new System.Numerics.Vector3(
                            (float)v.Position.X,
                            (float)v.Position.Y,
                            (float)0.0f
                        ));
                    }
                }

                ls.Add(new Polyline(ps, 1, c));
            }

            foreach (var pl in _dwg.Entities.OfType<Polyline2D>())
            {
                var c = new RGBA(pl.Color.R, pl.Color.G, pl.Color.B, 255);
                var ps = new List<System.Numerics.Vector3>();
                foreach (var v in pl.Vertices)
                {

                    if (transformation)
                    {
                        ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                            (float)v.Location.X,
                            (float)v.Location.Y,
                            (float)0.0f
                         )));
                    }
                    else
                    {
                        ps.Add(new System.Numerics.Vector3(
                            (float)v.Location.X,
                            (float)v.Location.Y,
                            (float)0.0f
                        ));
                    }


                }
                ls.Add(new Polyline(ps, pl.Thickness, c));
            }


            foreach (var pl in _dwg.Entities.OfType<Polyline3D>())
            {
                var c = new RGBA(pl.Color.R, pl.Color.G, pl.Color.B, 255);
                var ps = new List<System.Numerics.Vector3>();
                foreach (var v in pl.Vertices)
                {
                    if (transformation)
                    {
                        ps.Add(CoordinateTransformation(new System.Numerics.Vector3(
                            (float)v.Location.X,
                            (float)v.Location.Y,
                            (float)0.0f
                         )));
                    }
                    else
                    {
                        ps.Add(new System.Numerics.Vector3(
                            (float)v.Location.X,
                            (float)v.Location.Y,
                            (float)0.0f
                        ));
                    }

                }
                ls.Add(new Polyline(ps, 1, c));
            }


            return ls;
        }


        private IEnumerable<BlockReference> GetDxfBlockReferences(bool transformation = true)
        {
            var ls = new List<BlockReference>();

            foreach (var v in _dxf.Entities.Inserts)
            {
                var name = v.Block.Name;

                var pos = new System.Numerics.Vector3(
                       (float)v.Position.X,
                       (float)v.Position.Y,
                       (float)v.Position.Z
                   );

                if (transformation)
                {
                    pos = this.CoordinateTransformation(pos);
                }

                var scale = new System.Numerics.Vector3(
                        (float)v.Scale.X,
                        (float)v.Scale.Y,
                        (float)v.Scale.Z
                    );

                var b = new BlockReference(name, pos, (float)v.Rotation, scale);


                var ats = new List<AttributeDefine>();
                foreach (var attr in v.Attributes)
                {
                    var at = new AttributeDefine(attr.Tag, attr.Value);

                }

                b.Attributes = ats;

                ls.Add(b);
            }
            return ls;
        }



        private IEnumerable<BlockReference> GetDwgBlockReferences(bool transformation = true)
        {
            var ls = new List<BlockReference>();

            foreach (var v in _dwg.Entities.OfType<Insert>())
            {
                var name = v.Block.Name;

                var pos = new System.Numerics.Vector3(
                       (float)v.InsertPoint.X,
                       (float)v.InsertPoint.Y,
                       (float)v.InsertPoint.Z
                   );

                if (transformation)
                {
                    pos = this.CoordinateTransformation(pos);
                }

                var scale = new System.Numerics.Vector3(
                        (float)v.XScale,
                        (float)v.YScale,
                        (float)v.ZScale
                    );

                var b = new BlockReference(name, pos, (float)v.Rotation, scale);


                var ats = new List<AttributeDefine>();
                foreach (var attr in v.Attributes)
                {
                    var at = new AttributeDefine(attr.Tag, attr.Value);
                    ats.Add(at);
                }

                b.Attributes = ats;

                ls.Add(b);
            }
            return ls;
        }



        private IEnumerable<Block> GetDxfBlocks()
        {

            var bs = new List<Block>();

            foreach(var block in _dxf.Blocks)
            {
                var b = new Block(block.Name);
                foreach(var attr in block.AttributeDefinitions)
                {
                    b.Attributes.Add(new AttributeDefine(attr.Value.Tag,string.Empty));
                }
                bs.Add(b);
            }

            return bs;
        }


        private IEnumerable<Block> GetDwgBlocks()
        {
            var bs = new List<Block>();
            foreach (var block in _dwg.BlockRecords)
            {
                var b = new Block(block.Name);
                foreach (var attr in block.AttributeDefinitions)
                {
                    b.Attributes.Add(new AttributeDefine(attr.Tag, string.Empty));
                }
                bs.Add(b);
            }

            return bs;
        }


        #endregion
    }
}
