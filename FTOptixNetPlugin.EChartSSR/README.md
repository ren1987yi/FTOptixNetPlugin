# EChart Server Side Render

### Sample Code

- Init

```csharp
        var task = new LongRunningTask(() => {

            var jsfolder = string.Empty;
            if (!EChartSSR.Render.Instance.Inited)
            {
                var uri = ResourceUri.FromProjectRelativePath(string.Empty);
                var folder = uri.Uri;
                jsfolder = Path.Combine(folder, "NetSolution", "bin", "echartlib");
                //Log.Info("EchartSSR", $"js path:{jsfolder}");
                EChartSSR.Render.Instance.Init(jsfolder);

                Log.Info("EChartSSR", $"Init ok");

            }
            else
            {
                Log.Warning("EChartSSR", $"Init Error ,js path:{jsfolder}");
            }

        }, LogicObject);

        task.Start();
```



- Use Sample

```csharp
    svg = Owner as AdvancedSVGImage;
    //echart option
    var option = (string)vOption.Value;
    int width = vWidth.Value;
    int height = vHeight.Value;
    if (EChartSSR.Render.Instance.RenderObjString(width, height, option, out var content))
    {
        svg?.SetImageContent(content);
    }
```