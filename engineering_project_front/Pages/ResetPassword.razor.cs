using Blazored.SessionStorage;
using engineering_project.Models.Parameters;
using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace engineering_project_front.Pages
{
    public partial class ResetPassword
    {
        [Parameter]
        public string Code { get; set; } = string.Empty;
        private string email = string.Empty;
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


        protected async override Task OnInitializedAsync()
        {
            CreateTree();

            await base.OnInitializedAsync();
        }

        public void OnConfirmChangeClicked()
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

            if (resetPassword.ChangePassword(parameters))
                navManager.NavigateTo("/home");
        }

        private void CreateTree()
        {
            SidebarMenu.Instance.TreeData = new()
            {
                new TreeData
                {
                    Id = "1",
                    Name = "Ogólne",
                    HasChild = true,
                    Expanded = true,
                },
                new TreeData
                {
                    Id = "2",
                    Pid = "1",
                    Name = "Strona głowna",
                },
                new TreeData
                {
                    Id = "3",
                    Pid = "1",
                    Name = "Login"
                },
                new TreeData
                {
                    Id = "4",
                    Pid = "1",
                    Name = "Zarządzanie użytkownikami",
                },
                new TreeData
                {
                    Id = "5",
                    Pid = "1",
                    Name = "Zarządzanie zespołami",

                },
                new TreeData()
                {
                    Id= "6",
                    Pid = "1",
                    Name = "Zmień godziny pracy"
                }
            };

        }
    }
}
