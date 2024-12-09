using engineering_project_front.Models.Parameters;
using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
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
        [Inject]
        private IValidateRole validateRole { get; set; } = default!;
        #endregion

        #region Parameters
        [Parameter]
        public long? UserId { get; set; }

        List<RoleParameters> Roles = new List<RoleParameters>
        {
            new RoleParameters { ID = 0, Name = "Administrator" },
            new RoleParameters { ID = 1, Name = "Kierownik" },
            new RoleParameters { ID = 2, Name = "Pracownik" }
        };
        #endregion
        private UserRequest User { get; set; } = new UserRequest();
        private bool IsEditing => UserId.HasValue;
        private List<TeamsResponse> Teams { get; set; } = new List<TeamsResponse>();

        #region Toast
        private SfToast? Toast;
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Administrator"))
                NavManager.NavigateTo("/auth-error");

            var responseTeams = await TeamsService.GetTeamsAsync();
            if (responseTeams.Success)
            {
                Teams = responseTeams.Data;

            }
            else
            {
                ShowToast(responseTeams.Message, responseTeams.Success);
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
                    ShowToast(response.Message, response.Success);
                }
            }
        }


        #region ToastAndMapping
        private async Task ShowToast(string message, bool success)
        {
            Message = message;
            if (success)
            { Title = "Sukces!"; }
            else
            { Title = "Błąd!"; }
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
                    ShowToast(response.Message, response.Success);
                    await Task.Delay(2000);
                    NavManager.NavigateTo($"/UserDetails/{User.ID}");
                }
                else
                {
                    ShowToast(response.Message, response.Success);
                }
            }
            else
            {
                var response = await UsersService.AddUser(User);
                if (response.Success)
                {
                    ShowToast(response.Message, response.Success);
                    await Task.Delay(2000);
                    NavManager.NavigateTo("/UsersList");
                }
                else
                {
                    ShowToast(response.Message, response.Success);
                }
            }

        }

        private void Cancel()
        {
            if (IsEditing)
            {
                NavManager.NavigateTo($"/UserDetails/{User.ID}");
            }
            else
            {
                NavManager.NavigateTo("/UsersList");
            }
        }
    }
}
