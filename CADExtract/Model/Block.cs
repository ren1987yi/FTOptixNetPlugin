using ACadSharp.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace CADExtract.Model
{
    public class Block
    {
        public string Name { get; set; }

        public AttributeTable Attributes { get; set; }
        public byte[] Preview { get; set; }


        public Block()
        {

        }


        public Block(ACadSharp.Tables.BlockRecord block)
        {
            Name = block.Name;
            //Preview = new byte[block.Preview.LongLength];
            //block.Preview.CopyTo(Preview,block.Preview.LongLength) ;
        }


        public Block(netDxf.Blocks.BlockRecord block)
        {
            Name = block.Name;
            
        }

    }
}
