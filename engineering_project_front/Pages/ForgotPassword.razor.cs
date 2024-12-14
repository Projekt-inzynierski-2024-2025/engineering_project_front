using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class ForgotPassword
    {
        SfToast ToastObj = default!;
        private string ToastTitle = string.Empty;
        private string ToastContent = string.Empty;

        private string mail = string.Empty;

        private bool isButtonDisabled = false;

        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private IResetPassword resetPassword { get; set; } = default!;

        private async Task OnSendButtonClicked()
        {
            isButtonDisabled = true;
            if (mail == string.Empty)
            {
                ToastTitle = "Błąd";
                ToastContent = "Pole jest puste.";
                await InvokeAsync(StateHasChanged);
                await ToastObj.ShowAsync();
                isButtonDisabled = false;
                return;
            }

            await resetPassword.SendAskForResetCode(mail);

            ToastTitle = "Wysłano kod";
            ToastContent = "Jeżeli mail jest poprawny, kod powinien zostać wysłany na podaną skrzynkę pocztową.";
            await InvokeAsync(StateHasChanged);
            await ToastObj.ShowAsync();
            isButtonDisabled = false;
        }

        private void OnBackToLoginClicked()
        {
            navManager.NavigateTo("/");
        }
    }
}