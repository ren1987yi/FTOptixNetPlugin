namespace DaemonApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var p = System.Diagnostics.Process.GetCurrentProcess();
            var a = p.ProcessName;
            var bb = Directory.GetCurrentDirectory();


            Console.WriteLine(a);
            Console.WriteLine(bb);
        }
    }
}
