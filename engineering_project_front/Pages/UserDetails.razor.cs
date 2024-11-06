using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace engineering_project_front.Pages
{
    public partial class UserDetails : ComponentBase
    {


        [Inject]
        private IUsersService UsersService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Parameter]
        public required string ParamID { get; set; }

        private long ID => long.Parse(ParamID);
        private UsersResponse? User { get; set; } = new UsersResponse();


        protected async override Task OnInitializedAsync()
        {


            await GetUser();

            CreateTree();

            await base.OnInitializedAsync();
        }

        private async Task GetUser()
        {
            User = await UsersService.GetUser(ID);
        }

        private void EditUser()
        {
           
           NavManager.NavigateTo($"/add-edit-user/{User.ID}");
           
        }
        private void DeleteUser()
        {

             UsersService.DeleteUser(ID);

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

                }
            };
        }
    }
}
