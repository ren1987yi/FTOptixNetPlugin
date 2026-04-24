using FrontEnd_BlazorApp.Models;
using FrontEnd_BlazorApp.Models.JsFunc;
using Microsoft.AspNetCore.Components;

namespace FrontEnd_BlazorApp.Services
{
    public interface IGoLogixService
    {
        Task<JsConfig> GetJsConfig();
        Task<bool> SubmitJsConfig(JsConfig cfg);

        Task<bool> RestartJSVM();

        Task<HostInfo> GetHostState();

    }
    public class GoLogixService : IGoLogixService
    {
        readonly string BasicUrl = "http://localhost:49000";

        readonly HttpClient _httpClient;
        public GoLogixService(IConfiguration configuration)
        {
            var cc = configuration.GetValue<string>("ServerConfigure:GoLogix:Url", BasicUrl);
            BasicUrl = cc;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BasicUrl)
            };
        }

        public async Task<HostInfo> GetHostState()
        {
            return await _httpClient.GetFromJsonAsync<HostInfo>("/stat");
        }

        public async Task<JsConfig> GetJsConfig()
        {
            //throw new NotImplementedException();
            
            return await _httpClient.GetFromJsonAsync<JsConfig>("/jsfunc");

        }

        public async Task<bool> RestartJSVM()
        {
            var rr = await _httpClient.PostAsJsonAsync<int>("/restartjs",1);
            return rr.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> SubmitJsConfig(JsConfig cfg)
        {
            var rr = await _httpClient.PostAsJsonAsync<JsConfig>("/jsfunc", cfg);

            return rr.StatusCode == System.Net.HttpStatusCode.OK;

        }
    }
}
