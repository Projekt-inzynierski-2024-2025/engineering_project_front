﻿using engineering_project_front.Models.Parameters;
using engineering_project_front.Services.Interfaces;

using Microsoft.AspNetCore.Components;

using Syncfusion.Blazor.Notifications;

using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;

namespace engineering_project_front.Pages
{
    public partial class Login
    {
        private LoginParameters loginParameter = new();

        SfToast ToastObj = default!;
        private string ToastContent = string.Empty;

        private string login = string.Empty;
        private string password = string.Empty;
        private bool showPassword;


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
                ToastContent = "Login jest pusty.";
                await InvokeAsync(StateHasChanged);
                await ToastObj.ShowAsync();
                return;
            }

            if (loginParameter.Password == null)
            {
                ToastContent = "Hasło jest puste.";
                await InvokeAsync(StateHasChanged);
                await ToastObj.ShowAsync();
                return;
            }

            var token = await LoginService.Login(loginParameter);

            if (string.IsNullOrEmpty(token))
            {
                ToastContent = "Nie zalogowano.";
                await InvokeAsync(StateHasChanged);
                await ToastObj.ShowAsync();
                return;
            }

            NavManager.NavigateTo("/home");
        }


        public async void Enter(KeyboardEventArgs e)
        {
            if(e.Code== "Enter" || e.Code == "NumpadEnter")
                await OnLogInButtonClicked();
        }
        private void OnForgotPasswordClicked()
        {

            NavManager.NavigateTo($"/forgot-password");
        }
    }
}
