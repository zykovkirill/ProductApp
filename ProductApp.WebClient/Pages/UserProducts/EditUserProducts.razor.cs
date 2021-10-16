using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.WebClient.Pages.UserProducts
{
    public partial class EditUserProducts: ComponentBase
    {
        [Inject] IJSRuntime js { get; set; }

        protected override async Task OnAfterRenderAsync(bool fistRender)
        {
            string token = authenticationState.Result.User.FindFirst("AccessToken").Value; 
            await this.js.InvokeVoidAsync("window.P00340.start", token);

        }
    }
}
