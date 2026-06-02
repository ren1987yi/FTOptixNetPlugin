using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService
{
    public class AppData : SingletonBase<AppData>
    {


        public string WebRoot { get; private set; }

        public List<Attachment> Attachments { get; private set; } = new List<Attachment>();

        public LocalStorage LocalStorage { get; private set; } = new LocalStorage();


        public Configuration Configuration { get; set; }

        public AppData()
        {
            var folder = AssemblyDirectory;

            WebRoot = Path.Combine(folder, "InternalWebServiceContent");
        }


        private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

    }
}
