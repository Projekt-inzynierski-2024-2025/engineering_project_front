using Blazored.SessionStorage;
using engineering_project.Models.Parameters;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;
using System.Text.RegularExpressions;

namespace engineering_project_front.Pages
{
    public partial class ResetPassword
    {
        [Parameter]
        public string Code { get; set; } = string.Empty;
        private string oldPassword = string.Empty;
        private string password = string.Empty;
        private string confirmPassword = string.Empty;

        private bool showPasswords;

        string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{8,}$";

        #region Toast
        private SfToast Toast = new();
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion

        #region Injections
        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;

        [Inject]
        private IUsersService usersService { get; set; } = default!;

        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private IResetPassword resetPassword { get; set; } = default!;
        #endregion

        public async Task OnConfirmChangeClicked()
        {
            var token = await sessionStorage.GetItemAsStringAsync("token");

            var user = await usersService.GetUserFromToken(token);

            if (string.IsNullOrEmpty(oldPassword))
            {
                Title = "Błąd";
                Message = "Pole Stare Hasło nie może być puste.";
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                Title = "Błąd";
                Message = "Pole Nowe Hasło nie może być puste.";
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                Title = "Błąd";
                Message = "Pole Potwierdź Hasło nie może być puste.";
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            if (password.Length < 8 || !Regex.IsMatch(password,pattern))
            {
                Title = "Błąd";
                Message = "Hasło powinno zawierać co najmniej 8 znaków oraz co najmniej 1 dużą literę, 1 małą literę i cyfrę.";
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            if (password != confirmPassword)
            {
                Title = "Błąd";
                Message = "Hasła nie są zgodne.";
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                return;
            }

            ResetPasswordParameters parameters = new()
            {
                Email = user.Email!,
                OldPassword = oldPassword,
                NewPassword = password
            };

            if (await resetPassword.ChangePassword(parameters, token))
            {
                Title = "Sukces";
                Message = "Hasło zostało zmienione.";
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
                navManager.NavigateTo("/my-account");
            }
            else
            {
                Title = "Błąd";
                Message = "Nie udało się zmienić hasła.";
                await InvokeAsync(StateHasChanged);
                await Toast.ShowAsync();
            }
        }
    }
}
