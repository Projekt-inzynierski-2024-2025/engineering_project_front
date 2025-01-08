using engineering_project.Models.Parameters;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;
using System.Text.RegularExpressions;

namespace engineering_project_front.Pages
{
    public partial class ResetPasswordWithCode
    {
        [Parameter]
        public string Code { get; set; } = string.Empty;
        private string email = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;
        private bool showPasswords;

        string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{8,}$";
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private IResetPassword resetPassword { get; set; } = default!;

        #region Toast
        private SfToast Toast = new();
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion

        public async Task OnConfirmChangeClicked()
        {
            if (string.IsNullOrEmpty(email))
            {
                Title = "Błąd";
                Message = "Pole Email nie może być puste.";
                await Task.Delay(100);
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            if (!Regex.IsMatch(email, emailPattern))
            {
                Title = "Błąd";
                Message = "Podany email nie jest prawidłowy.";
                await Task.Delay(100);
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                Title = "Błąd";
                Message = "Pole Nowe Hasło nie może być puste.";
                await Task.Delay(100);
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                Title = "Błąd";
                Message = "Pole Potwierdź Hasło nie może być puste.";
                await Task.Delay(100);
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, pattern))
            {
                Title = "Błąd";
                Message = "Hasło powinno zawierać co najmniej 8 znaków oraz co najmniej 1 dużą literę, 1 małą literę i cyfrę.";
                await Task.Delay(100);
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            if (password != confirmPassword)
            {
                Title = "Błąd";
                Message = "Hasła nie są zgodne.";
                await Task.Delay(100);
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            ResetPasswordParameters parameters = new()
            {
                Code = Code,
                Email = email,
                NewPassword = password
            };

            if (await resetPassword.ChangePassword(parameters))
                navManager.NavigateTo("/");
            else
            {
                Title = "Błąd";
                Message = "Nie udało się zmienić hasła.";
                await Task.Delay(100);
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }
        }
    }
}
