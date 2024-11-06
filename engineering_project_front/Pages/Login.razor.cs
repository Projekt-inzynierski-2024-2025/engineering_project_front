using engineering_project_front.Models.Parameters;
using engineering_project_front.Services.Interfaces;

using Microsoft.AspNetCore.Components;

using Syncfusion.Blazor.Notifications;

using Blazored.SessionStorage;

namespace engineering_project_front.Pages
{
    public partial class Login
    {
        private LoginParameters loginParameter = new();

        SfToast ToastObj = new();
        private string ToastContent = string.Empty;

        private string login = string.Empty;
        private string password = string.Empty;

        [Inject]
        private ISessionStorageService SessionStorage { get; set; } = default!;

        [Inject]
        private ILoginService LoginService { get; set; } = default!;

        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        private async Task OnLogInButtonClicked()
        {
            loginParameter.Login = login;
            loginParameter.Password = password;

            if (loginParameter.Login == null)
            {
                ToastContent = "Login is empty.";
                await ToastObj.ShowAsync();
                return;
            }

            if (loginParameter.Password == null)
            {
                ToastContent = "Password is empty.";
                await ToastObj.ShowAsync();
                return;
            }

            var token = await LoginService.Login(loginParameter);

            if (string.IsNullOrEmpty(token))
            {
                ToastContent = "Did not log in.";
                await ToastObj.ShowAsync();
                return;
            }

            await SessionStorage.SetItemAsync("token", token);

            NavManager.NavigateTo("/home");
        }

        private void OnForgotPasswordClicked()
        {
            NavManager.NavigateTo($"/forgot-password");
        }
    }
}
