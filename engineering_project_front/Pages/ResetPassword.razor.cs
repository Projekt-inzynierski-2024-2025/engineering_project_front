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
        private string oldPassword = string.Empty;
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
            var token = await sessionStorage.GetItemAsStringAsync("token");

            var user = await usersService.GetUserFromToken(token);

            if(string.IsNullOrEmpty(oldPassword))

            if (string.IsNullOrEmpty(password))
                return;

            if (string.IsNullOrEmpty(confirmPassword))
                return;

            if (password != confirmPassword)
                return;

            ResetPasswordParameters parameters = new()
            {
                Email = user.Email!,
                OldPassword = oldPassword,
                NewPassword = password
            };

            if (await resetPassword.ChangePassword(parameters, token))
                navManager.NavigateTo("/my-account");
        }
    }
}
