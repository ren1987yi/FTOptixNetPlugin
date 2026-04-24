using FrontEnd_BlazorApp.Models;
using FrontEnd_BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace FrontEnd_BlazorApp.Pages.Dashboard.Workplace
{
    public partial class Index
    {
        private readonly EditableLink[] _links =
            {
            new EditableLink {Title = "工作台", Href = "/"},
            new EditableLink {Title = "JS Plug-In", Href = "/workshop/jsfunc"},
            new EditableLink {Title = "常规设置", Href = "/settings/general"},
         
        };

        private ActivitiesType[] _activities = { };
        private NoticeType[] _projectNotice = { };

        [Inject] public IProjectService ProjectService { get; set; }

        [Inject] public IGoLogixService GologixService { get; set; }


        private HostInfo _hostInfo = new(){ };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //_projectNotice = await ProjectService.GetProjectNoticeAsync();

            var cfg = await GologixService.GetJsConfig();
            var prjs = new List<NoticeType>();

            prjs.Add(new NoticeType()
            {
                Title = "趋势预测 (Trend Forecasting)",
                Description = "根据历史时序数据，判断变化趋势及突变。无需预先数据学习，不受少数异常值干扰。",
                Member = "趋势预测",
                Logo = "assets/golang.svg"

            });


            prjs.Add(new NoticeType()
            {
                Title = @"快速傅里叶变换 (FFT)",
                Description = @"Fast Fourier Transform,是快速计算序列的离散傅里叶变换（DFT）或其逆变换的方法。算法返回各个频率分量上的能量数值。",
                Member = @"快速傅里叶变换",
                Logo = "assets/golang.svg"
            });

            prjs.Add(new NoticeType()
            {
                Title = @"数据快照 (Data Snapshot)",
                Description = @"使用时序数据库，对PLC提交的数据进行记录，并可通过控制码进行数据持久化，为日后数据分析提供基础",
                Member = @"数据快照",
                Logo = "assets/golang.svg"
            });

            foreach (var func in cfg.Functions)
            {
                prjs.Add(new NoticeType()
                {
                    Title = func.FunctionName,
                    Description = func.Description,
                    Member = func.FunctionName,
                    Logo = "assets/js_brand.svg",
                    MemberLink = "/workshop/jsfunc/editor/" + func.FunctionName,
                    Href = "/workshop/jsfunc/editor/" + func.FunctionName,

                });
            }


            _projectNotice = prjs.ToArray();



            _activities = await ProjectService.GetActivitiesAsync();

            _hostInfo = await GologixService.GetHostState();

        }
    }
}