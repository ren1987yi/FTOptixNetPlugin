using Process.PIDLoader.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CADImportConfigure
{
    public partial class dlgBlock : Form
    {
        Block block;
        public dlgBlock(Block block)
        {
            InitializeComponent();
            this.block = block;
            UpdateUI();
        }

        private void UpdateUI()
        {
            txtBlockName.Text = block.Name;
            txtBindName.Text = block.BindName;
            dgMapper.DataSource = block.Attributes;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            block.BindName = txtBindName.Text;
            this.Close();
        }
    }
}
