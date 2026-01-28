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
    public partial class dlgElement : Form
    {
        IElement element;
        public dlgElement()
        {
            InitializeComponent();
        }

        public dlgElement(IElement element)
        {

            InitializeComponent();
            this.element = element;
            UpdateUI();
        }


        private void UpdateUI()
        {

            if(element is BlockReference)
            {
                printBlock(element as BlockReference);
            }else if(element is Polyline)
            {
                printPolyline(element as Polyline);
            }else if(element is Text)
            {
                printText(element as Text);
            }

        }


        private void printText(Text txt)
        {
            this.txtDetail.Text = @$"
TEXT : {txt.Value}
POSITION: {txt.Position.X},{txt.Position.Y}
ROTATION: {txt.Rotation}
";
        }
        private void printPolyline(Polyline polyline)
        {
            var text = "POINTS:\n";

            foreach(var p in polyline.Vertexes)
            {
                text += $"{p.X:f6},{p.Y:f6} \r\n";
            }

            this.txtDetail.Text = text;



        }


        private void printBlock(BlockReference block)
        {
            var txt = @$"
Block Name: {block.BlockName}
POSITION: {block.Position.X},{block.Position.Y}
ROTATION: {block.Rotation}
ATTRS:
";
            foreach(var attr in block.Attributes)
            {
                txt += $"{attr.Tag} = {attr.Value}\r\n";
            }
            this.txtDetail.Text = txt;
        }

    }
}
