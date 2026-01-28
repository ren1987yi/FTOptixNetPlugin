using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using System.Reflection;


namespace FTOptixNetPlugin.BarcodeSSR
{
    public sealed class Render
    {

        static readonly Render instance = new Render();
        static readonly V8ScriptEngine engine = new V8ScriptEngine();


        bool _inited;
        public bool Inited
        {
            get => _inited;

        }

        private Render() { 
        
        }



        public static Render Instance
        {
            get {

                return instance;
            }
        }



        private void init()
        {
            engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableAllLoading;

            engine.AddHostType(typeof(Console));

            engine.Execute(@"
            var barcode,qrcode,svg2url ;
");

            engine.Execute(new DocumentInfo() { Category = ModuleCategory.CommonJS }, @"

                barcode = require('barcode/index.js');
                qrcode = require('qrcode/index.js');
                svg2url = require('svg2url/index.js');
                
            ");

            engine.Execute(@"
function renderQrcode(content,padding,width,height,color,background,ecl){


var svgString = qrcode({
  content: content,
  padding: padding,
  width: width,
  height: height,
  color: color,
  background: background,
  ecl: ecl
})

return svgString;
}


function renderBarcode(content,type,barwidth,width,height,color,background,showHRI,fontSize){

    var _showHRI = false;
if(showHRI == 'true'){
    _showHRI = true;
}


    var setting = {
            width: width,
    barWidth: barwidth,
    barHeight: height,
    moduleSize: 1,
    showHRI: _showHRI,
    addQuietZone: false,
    marginHRI: 0,
    bgColor: background,
    color: color,
    fontSize: fontSize,
    output: ""svg"",
    posX: 0,
    posY: 0
  
};

    Console.WriteLine(setting);

    var svgString = barcode(content,type,setting);


    return svgString;


}



");
        }

        public void Init(string jsFolder = null)
        {
            

            if (string.IsNullOrWhiteSpace(jsFolder))
            {
                    var codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);

                var folder1 = Path.GetDirectoryName(path);

                var folder = Path.Combine(folder1, "JsBarcode");
                jsFolder = folder;
            }

            if (!_inited)
            {
                engine.DocumentSettings.SearchPath = jsFolder;
                init();
                _inited = true;
            }
        }

        public bool RenderQRCode(string content, out string svg, int padding=4,int width=256,int height=256,string color="#000000",string background = "#ffffff",string ecl="M")
        {
            svg = string.Empty;
            try
            {
                svg = engine.Script.renderQrcode(content,padding,width,height,color,background,ecl);
                return true;
            }
            catch (Exception ex) { 

                return false;
            }
        }


        public bool RenderBarCode(string content,out string svg,string type="code39", int barwidth = 1,int width = 100, int height = 50, string color = "#000000", string background = "#ffffff",bool showHRI = true,string fontSize = "1em")
        {


  //          {
  //          width: 100,
  //  barWidth: 1,
  //  barHeight: 50,
  //  moduleSize: 1,
  //  showHRI: false,
  //  addQuietZone: false,
  //  marginHRI: 0,
  //  bgColor: "transparent",
  //  color: "#000000",
  //  fontSize: 12,
  //  output: "svg",
  //  posX: 0,
  //  posY: 0
  //};
            svg = string.Empty;
            try
            {
                svg = engine.Script.renderBarcode(content,type, barwidth,width,height,color,background,showHRI.ToString().ToLower(),fontSize);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                

                return false;
            }
        }

    }
}
