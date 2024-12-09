using Blazored.SessionStorage;
using engineering_project.Models.Parameters;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace engineering_project_front.Pages
{
    public partial class ResetPassword
    {
        [Parameter]
        public string Code { get; set; } = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;

        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;

        [Inject]
        private IUsersService usersService { get; set; } = default!;

        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private IResetPassword resetPassword { get; set; } = default!;

        public async Task OnConfirmChangeClicked()
        {
            var token = sessionStorage.GetItemAsStringAsync("token").Result;

            if (token == null)
                return;
            token = token.Trim('"');

            var user = usersService.GetUserFromToken(token).Result;

            if (password != string.Empty)
                return;

            if (confirmPassword != string.Empty)
                return;

            if (password != confirmPassword)
                return;

            ResetPasswordParameters parameters = new()
            {
                Email = user.Email!,
                NewPassword = password
            };

            if (await resetPassword.ChangePassword(parameters))
                navManager.NavigateTo("/home");
        }
    }
}
