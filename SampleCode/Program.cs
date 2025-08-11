using FTOptixNetPlugin.IPP;
namespace SampleCode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");


            var filepath = @"C:\Users\YRen6\OneDrive - Rockwell Automation, Inc\Documents\Rockwell Automation\FactoryTalk Optix\ProjectTemplates\FTOptix_template_EdgeProjectTemplate\ProjectFiles\pdfs\25317000001381147015.pdf";

            var client = new Client(new Uri("ipp://10.108.164.27:631/ipp/print"));
            client.PrintJobStatusChanged += Client_PrintJobStatusChanged;
            var job = client.PrintFile(filepath, false, string.Empty, string.Empty);

            if(job.Status >= 0)
            {
                Console.WriteLine("job created");
            }
            else
            {
                Console.WriteLine("job error");
                return;
            }

            Console.ReadKey();
            client.PrintJobStatusChanged -= Client_PrintJobStatusChanged;

            Console.WriteLine("bye bye");
        }

        private static void Client_PrintJobStatusChanged(object? sender, PrintJobEventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine(e.Job.Status);

        }
    }
}
