using engineering_project_front.Models.Parameters;
using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class AddEditUser : ComponentBase
    {
        #region Injects
        [Inject]
        private IUsersService UsersService { get; set; } = default!;
        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        [Inject]
        private IValidateRole validateRole { get; set; } = default!;
        #endregion

        #region Parameters
        [Parameter]
        public string? UserId { get; set; }
        public long? UserID { get; set; }

        List<RoleParameters> Roles = new List<RoleParameters>
        {
            new RoleParameters { ID = 0, Name = "Administrator" },
            new RoleParameters { ID = 1, Name = "Kierownik" },
            new RoleParameters { ID = 2, Name = "Pracownik" }
        };
        #endregion
        private UserRequest User { get; set; } = new UserRequest();
        private bool IsEditing => !string.IsNullOrEmpty(UserId);
        private List<TeamsResponse> Teams { get; set; } = new List<TeamsResponse>();

        #region Toast
        private SfToast Toast = new();
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
                Teams = responseTeams.Data!;

            }
            else
            {
                await Task.Delay(100);
                await ShowToast(responseTeams.Message!, responseTeams.Success);
            }
            if (IsEditing)
            {
                try
                {
                    UserID = long.Parse(EncryptionHelper.Decrypt(UserId));
                }
                catch
                {
                    await Task.Delay(100);
                    await ShowToast("Niepoprawny identyfikator zespołu.", false);
                    NavManager.NavigateTo("/error"); // Przekierowanie na stronę błędu
                    return;
                }
                var response = await UsersService.GetUser((long)UserID!);
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
                    await Task.Delay(100);
                    await ShowToast(response.Message!, response.Success);
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
            await Toast.ShowAsync();
        }
        private void MapResponseToRequest(UsersResponse res)
        {

            User.ID = res.ID;
            User.FirstName = res.FirstName!;
            User.LastName = res.LastName!;
            User.Email = res.Email!;
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
                    await Task.Delay(100);
                    await ShowToast(response.Message!, response.Success);
                    await Task.Delay(2000);
                    var encryptedId = EncryptionHelper.Encrypt(User.ID.ToString());
                    NavManager.NavigateTo($"/UserDetails/{encryptedId}");
                }
                else
                {
                    await Task.Delay(100);
                    await ShowToast(response.Message!, response.Success);
                }
            }
            else
            {
                var response = await UsersService.AddUser(User);
                if (response.Success)
                {
                    await Task.Delay(100);
                    await ShowToast(response.Message!, response.Success);
                    await Task.Delay(2000);
                    NavManager.NavigateTo("/UsersList");
                }
                else
                {
                    await Task.Delay(100);
                    await ShowToast(response.Message!, response.Success);
                }
            }

        }

        private void Cancel()
        {
            if (IsEditing)
            {
                var encryptedId = EncryptionHelper.Encrypt(User.ID.ToString());
                NavManager.NavigateTo($"/UserDetails/{encryptedId}");
            }
            else
            {
                NavManager.NavigateTo("/UsersList");
            }
        }
    }
}
