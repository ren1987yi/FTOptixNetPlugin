using SharpIpp;
using SharpIpp.Models;
using SharpIpp.Protocol.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FTOptixNetPlugin.IPP
{
    public class Client
    {

        public event EventHandler<PrintJobEventArgs> PrintJobStatusChanged;


        public Uri PrinterUri { get; private set; }
        public Client(Uri printerUri)
        {
            PrinterUri = printerUri;
        }

        /// <summary>
        /// Print whole file
        /// </summary>
        /// <param name="filepath">print file path</param>
        /// <param name="color_mode">is color mode</param>
        /// <param name="doc_name">document name</param>
        /// <param name="job_name">print job name</param>
        public PrintJob PrintFile(string filepath, bool color_mode, string doc_name, string job_name)
        {
            if (!File.Exists(filepath))
            {
                return new PrintJob() { JobName = job_name, Status = PrintJobStatus.DocumentEmpty };
            }


            if (PrinterUri == null)
            {
                return new PrintJob() { JobName = job_name, Status = PrintJobStatus.PrinterClientError };

            }


            if (string.IsNullOrWhiteSpace(doc_name))
            {
                doc_name = Path.GetFileName(filepath);
            }

            if (string.IsNullOrWhiteSpace(job_name))
            {
                job_name = "FTOptixIPP_" + DateTime.Now.ToString();
            }

            try
            {

                var job = new PrintJob() { JobName = job_name, Status = PrintJobStatus.None };

                var task = Task.Run(async () =>
                {
                    var client = new SharpIppClient();
                    var filePath = filepath;
                    using var stream = File.Open(filePath, FileMode.Open);
                    var printJobRequest = new PrintJobRequest
                    {
                        Document = stream,
                        OperationAttributes = new PrintJobOperationAttributes()

                        {
                            PrinterUri = this.PrinterUri,
                            DocumentName = doc_name,
                            DocumentFormat = "application/octet-stream",
                            Compression = Compression.None,
                            DocumentNaturalLanguage = "en",
                            JobName = job_name,
                            IppAttributeFidelity = false,


                        },
                        JobTemplateAttributes = new JobTemplateAttributes()
                        {
                            Copies = 1,
                            MultipleDocumentHandling = MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                            Finishings = Finishings.None,
                            //PageRanges = new SharpIpp.Protocol.Models.Range[] { 

                            //new SharpIpp.Protocol.Models.Range(1, 1),
                            //},
                            Sides = Sides.OneSided,
                            NumberUp = 1,
                            OrientationRequested = Orientation.Portrait,
                            PrinterResolution = new Resolution(600, 600, ResolutionUnit.DotsPerInch),
                            PrintQuality = PrintQuality.Normal,
                            PrintColorMode = color_mode ? PrintColorMode.Color : PrintColorMode.Monochrome,

                        }
                    };

                    await Task.Delay(200);
                    if (PrintJobStatusChanged != null)
                    {
                        job.Status = PrintJobStatus.Running;
                        PrintJobStatusChanged(this, new PrintJobEventArgs() { Job = job });
                    }
                    try
                    {

                        var printJobresponse = await client.PrintJobAsync(printJobRequest);
                        if (PrintJobStatusChanged != null)
                        {
                            job.Status = PrintJobStatus.Success;
                            PrintJobStatusChanged(this, new PrintJobEventArgs() { Job = job });
                        }
                    }
                    catch (Exception ex)
                    {
                        if (PrintJobStatusChanged != null)
                        {
                            job.Status = PrintJobStatus.Error;
                            PrintJobStatusChanged(this, new PrintJobEventArgs() { Job = job });
                        }
                    }

                  

                });


                return job;

            }
            catch (Exception ex)
            {

                return new PrintJob() { JobName = job_name, Status = PrintJobStatus.Error };
            }
        }

    }
}
