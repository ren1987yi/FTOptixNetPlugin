namespace FrontEnd_BlazorApp
{
    public class ServerConfigure
    {
        public GoLogixServerOptions GoLogix { get; set; } = new GoLogixServerOptions();


    }

    public class GoLogixServerOptions
    {
        public string Url { get; set; } = "http://127.0.0.1:49000";
            
    }

}
