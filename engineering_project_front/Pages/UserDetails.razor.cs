using Blazored.SessionStorage;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class UserDetails : ComponentBase
    {
        #region Injection

        [Inject]
        private IUsersService UsersService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private ISessionStorageService SessionStorage { get; set; } = default!;
        [Inject]
        private IValidateRole validateRole { get; set; } = default!;

        #endregion


        [Parameter]
        public required string ParamID { get; set; }

        private long ID => long.Parse(ParamID);
        private UsersResponse? User { get; set; } = new UsersResponse();

        private long UserID { get; set; } = 0;

        #region ToastAndNotification
        private SfToast Toast = default!;
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        private bool IsDeleteDialogVisible { get; set; } = false;
        #endregion

        protected async override Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Administrator"))
                NavManager.NavigateTo("/auth-error");

            await GetUser();
            await GetActualUser();
            await base.OnInitializedAsync();
        }

        private async Task GetUser()
        {

            var response = await UsersService.GetUser(ID);

            if (response.Success)
            {
                User = response.Data;

            }
            else
            {
                await ShowToast(response.Message!, response.Success);
            }
        }

        #region ToastAndNotification
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

        private void ShowDeleteConfirmation()
        {
            IsDeleteDialogVisible = true;
        }

        private void CloseDeleteDialog()
        {
            IsDeleteDialogVisible = false;
        }
        #endregion


        private async Task EditUser()
        {
            if (UserID == ID)
            {
                await ShowToast("Nie masz uprawnień do edycji tego użytkownika", false);
                return;
            }
            NavManager.NavigateTo($"/add-edit-user/{User!.ID}");

        }

        private async Task ConfirmDelete()
        {
            if (UserID == ID)
            {
                await ShowToast("Nie masz uprawnień do edycji tego użytkownika", false);
                return;
            }

            IsDeleteDialogVisible = false;
            var response = await UsersService.DeleteUser(ID);

            if (response.Success)
            {
                User = new UsersResponse();
                await ShowToast(response.Message!, response.Success);
                NavManager.NavigateTo("/UsersList");
            }
            else
            {
                await ShowToast(response.Message!, response.Success);
            }
        }

        private async Task GetActualUser()
        {
            var token = await SessionStorage.GetItemAsStringAsync("token");


            token = token.Trim('"');

            var response = await UsersService.GetUserFromToken(token);

            UserID = response.ID;
        }
    }
}
