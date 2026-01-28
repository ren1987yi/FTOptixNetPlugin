using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Process.PIDLoader
{
    public struct BBox
    {
        public Vector3 LeftDown ; 
        public Vector3 RightUp;

        public float Width => RightUp.X - LeftDown.X;
        public float Height => RightUp.Y - LeftDown.Y;


        public float XMin => LeftDown.X;
        public float XMax => RightUp.X;
        public float YMin => LeftDown.Y;
        public float YMax => RightUp.Y;


        public BBox(Vector3 leftDown, Vector3 rightUp)
        {
            LeftDown = leftDown;
            RightUp = rightUp;
        }

        public BBox(Vector2 leftDown, Vector2 rightUp)
        {
            LeftDown = new Vector3( leftDown,0);
            RightUp = new Vector3( rightUp,0);
        }

    }
}
