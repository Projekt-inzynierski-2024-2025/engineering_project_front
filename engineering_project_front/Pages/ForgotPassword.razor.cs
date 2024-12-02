using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class ForgotPassword
    {
        SfToast ToastObj = new();
        private string ToastContent = string.Empty;

        private string mail = string.Empty;

        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private IResetPassword resetPassword { get; set; } = default!;

        private async Task OnSendButtonClicked()
        {
            if (mail == string.Empty)
            {
                ToastContent = "Login is empty.";
                await InvokeAsync(StateHasChanged);
                await ToastObj.ShowAsync();
                return;
            }

            await resetPassword.SendAskForResetCode(mail);

            ToastContent = "If e-mail is correct, you should receive code and link to reset your password.";
            await ToastObj.ShowAsync();
        }

        private void OnBackToLoginClicked()
        {
            navManager.NavigateTo("/");
        }
    }
}