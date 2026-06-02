using CommandLine;
using System.Drawing;
using System.Drawing.Imaging;

namespace CaptureScreen
{
    internal class Program
    {

        public class Options
        {
            [Option('x', "sx", Required = false,Default =0, HelpText = "Screen start x location")]
            public int X { get; set; }

            [Option('y', "sy", Required = false, Default =0, HelpText = "Screen start y location")]
            public int Y { get; set; }

            [Option('w', "width", Required = true, HelpText = "Screen width .")]
            public int Width { get; set; }


            [Option('h', "height", Required = true, HelpText = "Screen height .")]
            public int Height { get; set; }


            [Option('o', "output", Required = true, HelpText = "Output file path.")]
            public string Output { get; set; }
        }

        static void Main(string[] args)
        {

            Parser.Default.ParseArguments<Options>(args)
.WithParsed<Options>(opts =>
{
    Console.WriteLine($"X: {opts.X}");
    Console.WriteLine($"Y: {opts.Y}");
    Console.WriteLine($"Width: {opts.Width}");
    Console.WriteLine($"Height: {opts.Height}");
    Console.WriteLine($"Output: {opts.Output}");


    using var bitmap = new Bitmap(opts.Width, opts.Height);
    using (var g = Graphics.FromImage(bitmap))
    {
        g.CopyFromScreen(opts.X, opts.Y, 0, 0,
        bitmap.Size, CopyPixelOperation.SourceCopy);
    }
    bitmap.Save(opts.Output, ImageFormat.Png);



})
.WithNotParsed(errors =>
{
    Console.WriteLine("Invalid arguments.");
});



        }
    }
}
