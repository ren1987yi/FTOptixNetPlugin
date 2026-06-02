using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService
{
    public class Configuration
    {
        public string UploadFileFolder { get; set; }

        public string DownloadFileFolder { get => Path.Combine(AppData.Instance.WebRoot, downloadFolderName); }

        private string downloadFolderName = "downloads";
        public string DownloadFolderName { 
            
            
            get {
                return downloadFolderName;

            }

            set {

                downloadFolderName = value;


            }
        }
        public Action<string,string,string> OnUpload { get; set; }

    }
}
