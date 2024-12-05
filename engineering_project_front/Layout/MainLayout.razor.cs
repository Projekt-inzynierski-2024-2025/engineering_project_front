using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;

namespace engineering_project_front.Layout
{
    public partial class MainLayout
    {
        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;
        [Inject]
        private NavigationManager navManager { get; set; } = default!;
        protected async override Task OnInitializedAsync()
        {
            await CheckForToken();

            await base.OnInitializedAsync();
        }

        private async Task CheckForToken()
        {
            if (!await sessionStorage.ContainKeyAsync("token"))
                navManager.NavigateTo("/");
        }
    }
}
