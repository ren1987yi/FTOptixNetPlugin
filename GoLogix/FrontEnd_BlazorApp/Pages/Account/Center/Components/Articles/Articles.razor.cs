using FrontEnd_BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace FrontEnd_BlazorApp.Pages.Account.Center
{
    public partial class Articles
    {
        [Parameter] public IList<ListItemDataType> List { get; set; }
    }
}