using S7.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadPath
{
    public partial class DlgDownload : Form
    {

        CamProfile _camFile = null;

        public DlgDownload()
        {
            InitializeComponent();
        }

        public DlgDownload(CamProfile camFile)
        {
            InitializeComponent();
            _camFile = camFile;
        }

        private void btnDonwload_Click(object sender, EventArgs e)
        {
            if(_camFile == null)
            {
                MessageBox.Show("数据没有");
                return;
            }


            var plc = new Plc(CpuType.S71500, txtIP.Text, short.Parse(txtRack.Text), short.Parse(txtSlot.Text));
            try
            {
                plc.Open();

                if (!plc.IsConnected)
                {
                    MessageBox.Show("PLC 没有连接");
                    return;
                }
                var xName = txtXName.Text;
                var yName = txtYName.Text;

                var i = 0;
                foreach (var point in _camFile.Points)
                {
                    var xx = xName.Replace("*", i.ToString());
                    var yy = yName.Replace("*", i.ToString());

                    plc.Write(xx, point.X);
                    plc.Write(yy, point.Y);
                    i++;
                }



                plc.Close();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }
    }
}
