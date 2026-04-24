using FrontEnd_BlazorApp.Models.JsFunc;
using FrontEnd_BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FrontEnd_BlazorApp.Pages.Workshop.JsFunc
{
    public partial class Index
    {
        [Inject]
        public IGoLogixService goLogixService { get; set; }

        [Inject]
        public IMessageService messageService { get; set; }

        private JsFuncDef[] _data = { };
        JsConfig? _jsConfig;

        bool isLoading = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await OnRefresh();
        }


        bool _visible = false;
        bool _confirmLoading = false;
        Models.JsFunc.JsFuncDef _newModel = new JsFuncDef();
        private async Task AddJsFunc()
        {

            _newModel = new JsFuncDef();


            _visible = true;
        }


        private async Task OnAddHandleOk(MouseEventArgs e)
        {
            var d = _data.Where(c => c.Cmd == _newModel.Cmd || c.FunctionName == _newModel.FunctionName).Count();

            if(d > 0)
            {
                await messageService.ErrorAsync("Commmand Index and Name are already exists");
                return;
            }


            _confirmLoading = true;
            StateHasChanged();

            _jsConfig.Functions.Add(_newModel);
            await goLogixService.SubmitJsConfig(_jsConfig);

            await Task.Delay(2000);

            _visible = false;
            _confirmLoading = false;
            await OnRefresh();
        }

        private void OnAddHandleCancel(MouseEventArgs e)
        {
            Console.WriteLine("Clicked cancel button");
            _visible = false;
        }

        private async Task OnRefresh()
        {

            isLoading = true;

            var cfg = await goLogixService.GetJsConfig();
            _jsConfig = cfg;
            _data = _jsConfig.Functions.ToArray();
            isLoading = false;
        }


        bool _restarting = false;
        private async Task OnRestart()
        {
            _restarting = true;

            StateHasChanged();

            await goLogixService.RestartJSVM();
            await Task.Delay(2000);


            _restarting = false;

            StateHasChanged();
        }
    }
}
