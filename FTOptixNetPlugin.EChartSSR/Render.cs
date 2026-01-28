
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace FTOptixNetPlugin.EChartSSR
{


    public sealed class Render
    {
        //static readonly string jsFolder = @"D:\code\csharp\Test\DotNetEchartSSR\bin\Debug\net6.0\echarts\";
        static readonly Render instance = new Render();
        static readonly V8ScriptEngine engine = new V8ScriptEngine();

        bool _inited;
        public bool Inited
        {
            get => _inited;

        }

        private Render()
        {

        }

        public static Render Instance
        {
            get
            {

                //if (!instance.Inited) {

                //    var codeBase = Assembly.GetExecutingAssembly().Location;
                //    UriBuilder uri = new UriBuilder(codeBase);
                //    string path = Uri.UnescapeDataString(uri.Path);

                //    var folder1 = Path.GetDirectoryName(path);

                //    var folder = Path.Combine(folder1, "EChartSSR");
                //    instance.Init(folder);
                //}

                return instance;
            }
        }









        private void init()
        {
            engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableAllLoading;

            engine.AddHostType(typeof(Console));

            Action<ScriptObject, int> setTimeout = (func, delay) =>
            {
                var timer = new Timer(_ => func.Invoke(false));
                timer.Change(delay, Timeout.Infinite);
            };

            engine.Script._setTimeout = setTimeout;
            engine.Execute(@"
var echarts;

    function setTimeout(func, delay) {
        let args = Array.prototype.slice.call(arguments, 2);
        _setTimeout(func.bind(undefined, ...args), delay || 0);
    }

");

            engine.Execute(new DocumentInfo() { Category = ModuleCategory.CommonJS }, @"

                 echarts = require('echarts.min.js');
            ");

            engine.Execute(@"
    function renderChart(width,height,optionString){
    let chart = echarts.init(null, null, {
                  renderer: 'svg',
                  ssr: true,
                  width: width,
                  height: height,
                });
var option =JSON.parse(optionString);
chart.setOption(option);

let svgStr = chart.renderToSVGString();
return svgStr
}

 function renderChartObj(width,height,objString){
    let chart = echarts.init(null, null, {
                  renderer: 'svg',
                  ssr: true,
                  width: width,
                  height: height,
                });
var option =eval('(' + objString + ')');
chart.setOption(option);

let svgStr = chart.renderToSVGString();
return svgStr
}

");
        }

        public void Init(string jsFolder)
        {
            if (!_inited)
            {

                engine.DocumentSettings.SearchPath = jsFolder;
                init();
                _inited = true;
            }

        }


        public bool RenderObj(int width, int height, object option, out string svgString)
        {
            try
            {
                var ss = Newtonsoft.Json.JsonConvert.SerializeObject(option);

                svgString = engine.Script.renderChart(width, height, ss);
                return true;
            }
            catch (Exception ex)
            {
                svgString = null;
                return false;
            }
        }

        public bool RenderOptionString(int width, int height, string optionString, out string svgString)
        {
            try
            {
                svgString = engine.Script.renderChart(width, height, optionString);
                return true;
            }
            catch (Exception ex)
            {
                svgString = null;
                return false;
            }
        }


        public bool RenderObjString(int width, int height, string optionString, out string svgString)
        {
            try
            {
                svgString = engine.Script.renderChartObj(width, height, optionString);
                return true;
            }
            catch (Exception ex)
            {
                svgString = null;
                return false;
            }
        }


        public bool RenderOptionStringToSvgFile(int width, int height, string optionString, string filepath)
        {
            if (!Inited)
            {
                return false;
            }

            var result = RenderOptionString(width, height, optionString, out var svgContent);
            if (result)
            {
                File.WriteAllText(filepath, svgContent);
            }
            return result;
        }

        public bool RenderObjStringToSvgFile(int width, int height, string objString, string filepath)
        {
            if (!Inited)
            {
                return false;
            }

            var result = RenderObjString(width, height, objString, out var svgContent);
            if (result)
            {
                File.WriteAllText(filepath, svgContent);
            }
            return result;
        }


        public bool RenderObjToSvgFile(int width, int height, object option, string filepath)
        {
            if (!Inited)
            {
                return false;
            }

            var result = RenderObj(width, height, option, out var svgContent);
            if (result)
            {
                File.WriteAllText(filepath, svgContent);
            }
            return result;
        }
    }

}
