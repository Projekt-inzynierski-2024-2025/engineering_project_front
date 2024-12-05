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
        #endregion


        [Parameter]
        public required string ParamID { get; set; }

        private long ID => long.Parse(ParamID);
        private UsersResponse? User { get; set; } = new UsersResponse();

        #region ToastAndNotification
        private SfToast? Toast;
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        private bool IsDeleteDialogVisible { get; set; } = false;
        #endregion

        protected async override Task OnInitializedAsync()
        {
            await GetUser();

            await base.OnInitializedAsync();
        }

        private async Task GetUser()
        {
             
            var response= await UsersService.GetUser(ID);

            if (response.Success)
            {
                User = response.Data;

            }
            else
            {
                ShowToast(response.Message, response.Success);
            }
        }

        #region ToastAndNotification
        private async Task ShowToast(string message, bool success )
        {
            Message = message;
            if (success)
            { Title = "Sukces!"; }
            else
            { Title = "Błąd!"; }
            await InvokeAsync(StateHasChanged);
            await Toast?.ShowAsync();
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


        private void EditUser()
        {

            NavManager.NavigateTo($"/add-edit-user/{User.ID}");

        }

        private async Task ConfirmDelete()
        {
            IsDeleteDialogVisible = false;
            var response = await UsersService.DeleteUser(ID);

            if (response.Success)
            {
                User = new UsersResponse();
                ShowToast(response.Message, response.Success );
                NavManager.NavigateTo("/UsersList");
            }
            else
            {
                ShowToast(response.Message, response.Success );
            }
        }
    }
}
