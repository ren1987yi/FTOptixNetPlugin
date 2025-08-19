# NetServer


- TCP
- UDP
- Http
- Https
- WebSocket
- MVC




### Sample Code
#### MVC

```csharp
	var www_root = "./wwwroot";
	//logging configuration
	var config = new LogConfigure();

	config.AddColordConsole(FTOptixNetPlugin.NetServer.Mvc.LogLevel.Trace, FTOptixNetPlugin.NetServer.Mvc.LogLevel.Fatal);
	config.AddFile(FTOptixNetPlugin.NetServer.Mvc.LogLevel.Trace, FTOptixNetPlugin.NetServer.Mvc.LogLevel.Fatal,"log.txt");

	//
	var app = new WebApplication(System.Net.IPAddress.Any, 49000, string.Empty, www_root, TimeSpan.FromSeconds(3600), config);
	//use routing
	app.UseRouting();
	//use static files
	app.UseStaticFile();
	//run web application
	app.Run();
```