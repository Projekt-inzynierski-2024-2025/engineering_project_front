using engineering_project.Models.Parameters;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace engineering_project_front.Pages
{
    public partial class ResetPasswordWithCode
    {
        [Parameter]
        public string Code { get; set; } = string.Empty;
        private string email = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;

        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private IResetPassword resetPassword { get; set; } = default!;

        public void OnConfirmChangeClicked()
        {
            //TO DO - Uładnij to
            if (email != string.Empty)
                return;

            if (password != string.Empty)
                return;

            if (confirmPassword != string.Empty)
                return;

            if (password != confirmPassword)
                return;

            ResetPasswordParameters parameters = new()
            {
                Code = Code,
                Email = email,
                NewPassword = password
            };

            if (resetPassword.ChangePassword(parameters))
                navManager.NavigateTo("/");
        }
    }
}
