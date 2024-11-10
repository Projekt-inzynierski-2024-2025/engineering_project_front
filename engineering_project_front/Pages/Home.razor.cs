using Blazored.SessionStorage;

using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;

using Microsoft.AspNetCore.Components;

namespace engineering_project_front.Pages
{
    public partial class Home
    {
        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;

        [Inject]
        private IUsersService usersService { get; set; } = default!;

        private string firstName = "<FIRST_NAME_PH>";
        private string lastName = "<LAST_NAME_PH>";
        private string email = "<EMAIL_PH>";
        private string team = "<TEAM_PH>";
        private string manager = "<MANAGER_PH>";
        private string role = "<ROLE_PH>";

        protected async override Task OnInitializedAsync()
        {
            CreateTree();

            await GetUser();

            await base.OnInitializedAsync();
        }

        private async Task GetUser()
        {
            var token = await sessionStorage.GetItemAsStringAsync("token");

            if (token == null)
                return;

            token = token.Trim('"');

            var user = await usersService.GetUserFromToken(token);

            firstName = user.FirstName!;
            lastName = user.LastName!;
            email = user.Email!;
            team = user.TeamName!;
            manager = user.Manager!;
            role = user.RoleName;
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
                    Selected = true
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
                new TreeData
                {
                    Id = "6",
                    Pid = "1",
                    Name = "Grafik",
                }
            };
        }
    }
}
