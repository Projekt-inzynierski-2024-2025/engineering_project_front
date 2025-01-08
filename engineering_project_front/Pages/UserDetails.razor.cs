using Blazored.SessionStorage;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services;
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
        public string ParamID { get; set; } = string.Empty;

        private long ID { get; set; }
        private UsersResponse? User { get; set; } = new UsersResponse();

        private long UserID { get; set; } = 0;

        #region ToastAndNotification
        private SfToast Toast = new();
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        private bool IsDeleteDialogVisible { get; set; } = false;
        #endregion

        protected async override Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Administrator"))
                NavManager.NavigateTo("/auth-error");

          
            if(!string.IsNullOrEmpty(ParamID))
            {
                try
                {
                    ID = long.Parse(EncryptionHelper.Decrypt(ParamID));
                }catch
                {
                    await Task.Delay(100);
                    await ShowToast("Niepoprawny identyfikator użytkownika.", false);
                    NavManager.NavigateTo("/error"); // Przekierowanie na stronę błędu
                    return;
                }
            }
            

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
                await Task.Delay(100);
                await ShowToast(response.Message!, response.Success);
            }
        }

        #region ToastAndNotification
        private async Task ShowToast(string message, bool success)
        {
            Message = message;
            Title = success ? "Sukces!" : "Błąd!";
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
                await Task.Delay(100);
                await ShowToast("Nie masz uprawnień do edycji tego użytkownika", false);
                return;
            }
            var encryptedId = EncryptionHelper.Encrypt(User!.ID.ToString());
            NavManager.NavigateTo($"/add-edit-user/{encryptedId}");

        }

        private async Task ConfirmDelete()
        {
            if (UserID == ID)
            {
                await Task.Delay(100);
                await ShowToast("Nie masz uprawnień do edycji tego użytkownika", false);
                return;
            }

            IsDeleteDialogVisible = false;
            var response = await UsersService.DeleteUser(ID);

            if (response.Success)
            {
                User = new UsersResponse();
                await Task.Delay(100);
                await ShowToast(response.Message!, response.Success);
                NavManager.NavigateTo("/UsersList");
            }
            else
            {
                await Task.Delay(100);
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
