using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class AddEditUser : ComponentBase
    {
        #region Injects
        [Inject]
        private IUsersService UsersService { get; set; }
        [Inject]
        private ITeamsService TeamsService { get; set; }
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        #endregion


        [Parameter]
        public long? UserId { get; set; }

        private UserRequest User { get; set; } = new UserRequest();
        private bool IsEditing => UserId.HasValue;
        private List<TeamsResponse> Teams { get; set; } = new List<TeamsResponse>();

        #region Toast
        private SfToast? Toast;
        private string Message { get; set; } = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            CreateTree();

           
            var responseTeams = await TeamsService.GetTeamsAsync();
            if (responseTeams.Success)
            {
                Teams = responseTeams.Data;

            }
            else
            {
                ShowToast(responseTeams.Message);
            }
            if (IsEditing)
            {
                
                var response = await UsersService.GetUser((long)UserId);
                if (response.Success)
                {
                    var user = response.Data;
                    if (user != null)
                    {
                        MapResponseToRequest(user);
                    }
                }
                else
                {
                    ShowToast(response.Message);
                }
            }
        }


        #region ToastAndMapping
        private async Task ShowToast(string message)
        {
            Message = message;
            await InvokeAsync(StateHasChanged);
            await Toast?.ShowAsync();
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
        #endregion


        private async Task HandleValidSubmit()
        {
            if (IsEditing)
            {
                var response = await UsersService.EditUser(User);
                if (response.Success)
                {
                    ShowToast(response.Message);
                    await Task.Delay(2000);
                    NavManager.NavigateTo("/UsersList");
                }
                else
                {
                    ShowToast(response.Message);
                }
            }
            else
            {
                var response = await UsersService.AddUser(User);
                if (response.Success)
                {
                    ShowToast(response.Message);
                    await Task.Delay(2000);
                    NavManager.NavigateTo("/UsersList");
                }
                else
                {
                    ShowToast(response.Message);
                }
            }
            
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
