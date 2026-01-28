namespace Process.PIDLoader.Elements
{
    public struct RGBA
    {
        private byte r;
        private byte g;
        private byte b;
        private byte alpha;
        
        public byte R { get => r; set => r = value; }
        public byte G { get => g; set => g = value; }
        public byte B { get => b; set => b = value; }
        public byte A { get => alpha; set => alpha = value; }
        public RGBA(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.alpha = a;
        }

        public RGBA()
        {

        }
    }
}
