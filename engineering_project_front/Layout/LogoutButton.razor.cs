using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;

namespace engineering_project_front.Layout
{
    public partial class LogoutButton
    {
        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;
        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        private void OnLogoutClicked()
        {
            sessionStorage.RemoveItemAsync("token");

            navManager.NavigateTo("/");
        }
    }
}
