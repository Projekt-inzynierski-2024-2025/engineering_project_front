using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace engineering_project_front.Pages
{
    public partial class AddEditUser : ComponentBase
    {
        [Inject]
        private IUsersService UsersService { get; set; }
        [Inject]
        private ITeamsService TeamsService { get; set; }
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;



        [Parameter]
        public long? UserId { get; set; }

        private UserRequest User { get; set; } = new UserRequest();
        private bool IsEditing => UserId.HasValue;
        private List<TeamsResponse> Teams { get; set; } = new List<TeamsResponse>();

        protected override async Task OnInitializedAsync()
        {
            CreateTree();

            Teams = await TeamsService.GetTeamsAsync();

            if (IsEditing)
            {
                var response = await UsersService.GetUser((long)UserId);
                if (response != null)
                {

                    MapResponseToRequest(response);
                }
            }
        }


        private void MapResponseToRequest(UsersResponse res)
        {

            User.ID = res.ID;
            User.FirstName = res.FirstName;
            User.LastName = res.LastName;
            User.Email = res.Email;
            User.Role = res.Role;
            User.TeamID = res.TeamID;

        }

        private async Task HandleValidSubmit()
        {
            if (IsEditing)
            {
                await UsersService.EditUser(User);
            }
            else
            {
                await UsersService.AddUser(User);
            }
            NavManager.NavigateTo("/UsersList");
        }

        private void Cancel()
        {
            NavManager.NavigateTo("/UsersList");
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
