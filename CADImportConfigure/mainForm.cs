
using Process.PIDLoader;
using Process.PIDLoader.Elements;
using System.Diagnostics;
using System.Security.Cryptography;


namespace CADImportConfigure
{
    public partial class mainForm : Form
    {

        ProjectDocument prj = new ProjectDocument();



        public mainForm()
        {

            InitializeComponent();


        }

        private void mainForm_Load(object sender, EventArgs e)
        {

        }

        private void MenuItem_OpenDxf_Click(object sender, EventArgs e)
        {


            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "DXF Files (.dxf)|*.dxf";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var filepath = dlg.FileName;
                    this.lbCadFilePath.Text = filepath;
                    prj.CADFilePath = filepath;
                    loadDxf(filepath);
                }

            }
        }


        private void MenuItem_OpenDwg_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "DWG Files (.dwg)|*.dwg";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var filepath = dlg.FileName;
                    //var filepath = @"D:\Work\Projects\PID Loader\2024-010Pb20240910.dwg";
                    prj.CADFilePath = filepath;
                    this.lbCadFilePath.Text = filepath;
                    loadDwg(filepath);
                }
            }
        }

        private void processDocument(Document doc)
        {
            var inserts = doc.GetBlockReferences(false);
            var blocks = doc.GetBlocks();

            var refblocks = inserts.Select(i => i.BlockName).Intersect(blocks.Select(c => c.Name));


            var _blocks_ = from block in blocks
                           join name in refblocks
                           on block.Name equals name
                           select block;

            _blocks = _blocks_.ToList();


            var els = new List<IElement>();
            els.AddRange(doc.GetTexts(false));
            els.AddRange(doc.GetPolylines(false));
            els.AddRange(doc.GetBlockReferences(false));

            _elements = els.ToList();



            prj.Blocks = _blocks;
            prj.Elements = _elements;
        }

        private void updateUI()
        {

            dgBlocks.DataSource = prj.Blocks;
            dgEntites.DataSource = prj.Elements;

            txtXmax.Text = $"{prj.XMax:f6}";
            txtYmax.Text = $"{prj.YMax:f6}";
            txtXmin.Text = $"{prj.XMin:f6}";
            txtYmin.Text = $"{prj.YMin:f6}";

            chbIgnoreNoBindBlock.Checked = prj.IgnoreNoBindBlock;
            chbBlock.Checked = prj.DrawBlock;
            chbText.Checked = prj.DrawText;
            chbLine.Checked = prj.DrawLine;


        }


        private void storeUI()
        {
            prj.IgnoreNoBindBlock = chbIgnoreNoBindBlock.Checked;
            prj.DrawBlock = chbBlock.Checked;
            prj.DrawText = chbText.Checked;
            prj.DrawLine = chbLine.Checked;

            prj.XMin = Convert.ToSingle(txtXmin.Text);
            prj.YMin = Convert.ToSingle(txtYmin.Text);

            prj.XMax = Convert.ToSingle(txtXmax.Text);
            prj.YMax = Convert.ToSingle(txtYmax.Text);

        }


        private void loadDwg(string filepath)
        {
            var doc = Document.LoadDwg(filepath, true, out var output);

            //dgBlocks.Rows.Clear();
            //dgEntites.Rows.Clear();
            processDocument(doc);
            updateUI();

        }



        List<Block> _blocks;
        List<IElement> _elements;
        private void loadDxf(string filepath)
        {

            var doc = Document.LoadDxf(filepath, true, out var output);

            //dgBlocks.Rows.Clear();
            //dgEntites.Rows.Clear();
            processDocument(doc);
            updateUI();
            //var inserts = doc.GetBlockReferences();
            //var blocks = doc.GetBlocks();

            //var refblocks = inserts.Select(i => i.BlockName).Intersect(blocks.Select(c => c.Name));




            //var _blocks_ = from block in blocks
            //              join name in refblocks
            //              on block.Name equals name
            //              select block;

            //_blocks = _blocks_.ToList();
            //dgBlocks.DataSource = _blocks;


            //var els = new List<IElement>();
            //els.AddRange(doc.GetTexts(false));
            //els.AddRange(doc.GetPolylines(false));
            //els.AddRange(doc.GetBlockReferences(false));
            //_elements = els.ToList();
            //dgEntites.DataSource = _elements;

        }

        private void Block_DoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var block = prj.Blocks[e.RowIndex];
            var dlg = new dlgBlock(block);
            dlg.ShowDialog();
            dgBlocks.DataSource = null;
            dgBlocks.DataSource = prj.Blocks;
        }

        private void Entites_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            var el = prj.Elements[e.RowIndex];
            var dlg = new dlgElement(el);
            dlg.ShowDialog();
            dgEntites.DataSource = null;
            dgEntites.DataSource = prj.Elements;
        }

        private void MenuItem_SaveProject_Click(object sender, EventArgs e)
        {


            var dlg = new SaveFileDialog();

            dlg.Filter = "Project Files (.prj)|*.prj";

            if (dlg.ShowDialog() == DialogResult.OK)
            {

                storeUI();
                var setting = new Newtonsoft.Json.JsonSerializerSettings();
                setting.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                var t = Newtonsoft.Json.JsonConvert.SerializeObject(prj, Newtonsoft.Json.Formatting.Indented, setting);

                File.WriteAllText(dlg.FileName, t);
            }


            //var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectDocument>(t,setting);

            //prj = obj;
            //updateUI();
        }

        private void MenuItem_OpenProject_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Project Files (.prj)|*.prj";

                if(dlg.ShowDialog() == DialogResult.OK)
                {

                    //var t = File.ReadAllText(dlg.FileName);
                    //var setting = new Newtonsoft.Json.JsonSerializerSettings();
                    //setting.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                    //var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectDocument>(t, setting);
                    var obj = ProjectDocument.LoadFile(dlg.FileName);
                    prj = obj;
                    updateUI();
                    this.lbCadFilePath.Text = prj.CADFilePath;
                    this.lbProjectPath.Text = dlg.FileName;
                }
            }
        }



    }
}
