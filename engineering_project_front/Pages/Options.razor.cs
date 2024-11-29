using Blazored.SessionStorage;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Data;

namespace engineering_project_front.Pages
{
    public partial class Options
    {
        [Inject]
        private NavigationManager navManager { get; set; } = default!;
        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;
        [Inject]
        private IUsersService usersService { get; set; } = default!;

        private long ID { get; set; }
        private string firstName { get; set; } = string.Empty;
        private string lastName { get; set; } = string.Empty;
        private string email { get; set; } = string.Empty;
        private string team { get; set; } = string.Empty;
        private string manager { get; set; } = string.Empty;
        private string role {  get; set; } = string.Empty;

        protected async override Task OnInitializedAsync()
        {
            await GetUser();

            await base.OnInitializedAsync();
        }

        private void onChangePasswordClicked() => navManager.NavigateTo("/reset-password");

        private async Task GetUser()
        {
            var token = await sessionStorage.GetItemAsStringAsync("token");

            if (token == null)
                return;

            token = token.Trim('"');

            var user = await usersService.GetUserFromToken(token);

            ID = user.ID!;
            firstName = user.FirstName!;
            lastName = user.LastName!;
            email = user.Email!;
            team = user.TeamName!;
            manager = user.Manager!;
            role = user.RoleName;
        }
    }
}
