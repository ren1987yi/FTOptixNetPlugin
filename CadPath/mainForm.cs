using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.IO;
using Svg;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Security.Cryptography;

namespace CadPath
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();



        }

        private void openDwgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "*.dwg|*.dwg";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var filepath = dlg.FileName;
                    loadDwg(filepath);
                }

            }





        }



        const float min_distance = 1.0f;

        private void btnSortLine_Click(object sender, EventArgs e)
        {
            sortLines();

            return;

        }


        private void sortLines()
        {
            sortTexts.Clear();

            var startPoint = Vector2.Zero;


            foreach (var p in _paths)
            {
                foreach (var s in p.Segments)
                {
                    s.Index = -1;
                }
            }

            var paths = _paths.OrderBy(c => c.Index).ToList();

            var segIndex = 0;
            foreach (var path in paths.OrderBy(c => c.Index))
            {

                while (path.Segments.Where(s => s.Index <= 0).Count() > 0)
                {
                    segIndex++;
                    var c1 = path.Segments.Where(s => s.Index <= 0).Count();
                    foreach (var segment in path.Segments.Where(s => s.Index <= 0))
                    {
                        if ((segment.Points.First() - startPoint).Length() <= min_distance)
                        {
                            segment.Index = segIndex;
                            startPoint = segment.Points.Last();


                            foreach (var p in segment.Points)
                            {
                                var dt = new DrawTextData()
                                {
                                    Text = segIndex.ToString(),
                                    Location = p
                                };
                                sortTexts.Add(dt);

                            }
                            break;
                        }
                        else if ((segment.Points.Last() - startPoint).Length() <= min_distance)
                        {
                            segment.Index = segIndex;
                            segment.Points.Reverse();
                            startPoint = segment.Points.Last();

                            foreach (var p in segment.Points)
                            {
                                var dt = new DrawTextData()
                                {
                                    Text = segIndex.ToString(),
                                    Location = p
                                };
                                sortTexts.Add(dt);

                            }
                            break;
                        }
                        else
                        {
                            //throw new Exception("点断开了");
                            continue;
                        }
                    }
                    var c2 = path.Segments.Where(s => s.Index <= 0).Count();
                    if (c1 == c2)
                    {
                        sortTexts.Clear();
                        //throw new Exception("点断开了");
                        MessageBox.Show("点断开，没有连续", "错误");
                        return;
                    }

                }

            }

            _sortedPaths = paths.ToList();

            var cam = new CamProfile();
            var camPoints = new List<Vector2>();
            var lastPoint = new Vector2(float.MaxValue);
            foreach (var path in paths.OrderBy(c => c.Index))
            {

                for (var i = 0; i < path.Count; i++)
                {
                    foreach (var segment in path.Segments.OrderBy(c => c.Index))
                    {
                        if ((segment.Points.First() - lastPoint).Length() < 1)
                        {
                            camPoints.AddRange(segment.Points.Skip(1));
                        }
                        else
                        {
                            camPoints.AddRange(segment.Points);
                        }

                        lastPoint = segment.Points.Last();

                    }
                }
            }
            cam.Points = camPoints.Select(p => new CamPoint() { X = p.X, Y = p.Y }).ToList();
            dvPoints.DataSource = null;
            dvPoints.DataSource = cam.Points;
            curCam = cam;
            cadView.Refresh();
        }

        CamProfile curCam;
        private void btnZoom_Click(object? sender, EventArgs e)
        {
            if (float.TryParse(txtZoom.Text, out var ff))
            {
                _zoom = ff;
                this.cadView.Refresh();
            }

        }

        private void btnZoomIn_Click(object? sender, EventArgs e)
        {
            _zoom += 100;
            this.cadView.Refresh();
        }

        private void btnZoomOut_Click(object? sender, EventArgs e)
        {
            _zoom -= 100;
            this.cadView.Refresh();
        }

        private void cadView_Resize(object sender, EventArgs e)
        {
            base.OnResize(e);

        }



        List<DrawTextData> sortTexts = new List<DrawTextData>();

        private void cadView_OnPaint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(System.Drawing.Color.Black);



            var zoom = _zoom / 100.0f;
            Vector2 pc = new Vector2(cadView.Width / 2, cadView.Height / 2);

            if (_polylines != null)
            {
                foreach (var pl in _polylines)
                {
                    var pen = new Pen(pl.Color, 1);
                    var ps = pl.Lines.Select(p => new PointF(((p * zoom).X + pc.X), (pc.Y - (p * zoom).Y))).ToArray();
                    g.DrawLines(pen, ps);
                }
            }

            if (sortTexts.Count > 0)
            {

                var brsh = new SolidBrush(System.Drawing.Color.Yellow);
                foreach (var t in sortTexts)
                {

                    var p = new PointF(((t.Location.X * zoom) + pc.X), (pc.Y - (t.Location.Y * zoom)));

                    g.DrawString(t.Text, SystemFonts.DefaultFont, brsh, p.X, p.Y);

                }
            }
        }


        List<DrawPolyLineData> _polylines;
        List<Path> _paths, _sortedPaths;
        float _zoom = 100.0f;
        //double minX,minY,maxX,maxY;

        private void loadDwg(string filepath)
        {


            //var svg = new Svg.SvgDocument();

            //svg.ViewBox = new Svg.SvgViewBox();
            //var svgGroup = new SvgGroup();
            //svg.Children.Add(svgGroup);

            _zoom = 100.0f;

            //minX = double.MaxValue;
            //minY = double.MaxValue;
            //maxX = double.MinValue;
            //maxY = double.MaxValue;
            _polylines = new List<DrawPolyLineData>();
            CadDocument dwg;
            using (DwgReader reader = new DwgReader(filepath))
            {
                dwg = reader.Read();

            }

            convertToSvg(dwg);

            var paths = new List<Path>();

            CSMath.BoundingBox bbox = new CSMath.BoundingBox(0, 0, 0, 0, 0, 0);

            foreach (var entity in dwg.Entities)
            {
                switch (entity.ObjectType)
                {
                    case ObjectType.LWPOLYLINE:
                    case ObjectType.POLYLINE_2D:

                        var path = paths.Where(p => p.Name == entity.Layer.Name).FirstOrDefault();
                        if (path == null)
                        {
                            path = new Path()
                            {
                                Name = entity.Layer.Name,
                            };
                            paths.Add(path);
                            path.Index = paths.Count;
                        }

                        var seg = new Segment()
                        {
                            //Entity = entity,
                            Index = -1
                        };
                        path.Segments.Add(seg);

                        var box = entity.GetBoundingBox();
                        bbox = box.Merge(bbox);

                        //Svg.SvgPath svgline = new Svg.SvgPath();


                        System.Drawing.Color color = System.Drawing.Color.White;
                        if (entity.Color.IsByLayer)
                        {
                            var layer_color = dwg.Layers.Where(l => l.Name == entity.Layer.Name).FirstOrDefault().Color;
                            color = System.Drawing.Color.FromArgb(layer_color.R, layer_color.G, layer_color.B);
                        }

                        var polyline = new DrawPolyLineData();
                        polyline.Color = color;

                        polyline.Lines = getPolyLinePoints(entity);
                        seg.Points = getPolyLinePoints(entity);


                        _polylines.Add(polyline);

                        break;
                }
            }

            //svg.ViewBox = new Svg.SvgViewBox((float)bbox.Min.X, (float)bbox.Max.Y, (float)bbox.LengthX, (float)bbox.LengthY);

            _paths = paths;


            dvPath.DataSource = null;
            dvPath.DataSource = _paths;

            this.cadView.Refresh();




        }



        private List<Vector2> getPolyLinePoints(ACadSharp.Entities.Entity entity)
        {
            switch (entity.ObjectType)
            {
                case ObjectType.LWPOLYLINE:
                    var lw = entity as LwPolyline;
                    return lw.Vertices.Select(v => new Vector2((float)v.Location.X, (float)v.Location.Y)).ToList();
                    break;


                case ObjectType.POLYLINE_2D:
                    var pl = entity as Polyline2D;
                    return pl.Vertices.Select(v => new Vector2((float)v.Location.X, (float)v.Location.Y)).ToList();
                    break;
                default:
                    return null;
                    break;



            }


        }





        private void convertToSvg(CadDocument dwg)
        {
            var helper = new SvgHelper();
            helper.FromDwg(dwg);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            var dlg = new DlgDownload(this.curCam);
            dlg.ShowDialog();
        }
    }
}
