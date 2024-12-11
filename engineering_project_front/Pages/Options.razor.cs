using Blazored.SessionStorage;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;

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

        private UsersResponse? User = new();

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

            User!.ID = user.ID!;
            User!.FirstName = user.FirstName!;
            User!.LastName = user.LastName!;
            User!.Email = user.Email!;
            User!.TeamName = user.TeamName!;
            User!.Manager = user.Manager!;
            User!.Role = user.Role;
        }
    }
}
