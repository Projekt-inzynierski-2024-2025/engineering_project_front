using engineering_project_front.Models.Responses;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class TeamDetails : ComponentBase
    {
        #region Injects
        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private IValidateRole validateRole { get; set; } = default!;
        #endregion

        [Parameter]
        public string ParamID { get; set; } = string.Empty;

        private long ID { get; set; }
        private TeamsResponse? Team { get; set; } = new TeamsResponse();

        #region ToastAndNotification
        private SfToast Toast = new();
        private bool IsDeleteDialogVisible { get; set; } = false;
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            await GetTeam();
            await base.OnInitializedAsync();
        }

        private async Task GetTeam()
        {
            if (!await validateRole.IsAuthorized("Administrator"))
                NavManager.NavigateTo("/auth-error");


            if (!string.IsNullOrEmpty(ParamID))
            {
                try
                {
                    ID = long.Parse(EncryptionHelper.Decrypt(ParamID));
                }
                catch
                {
                    await Task.Delay(100);
                    await ShowToast("Niepoprawny identyfikator użytkownika.", false);
                    NavManager.NavigateTo("/error"); // Przekierowanie na stronę błędu
                    return;
                }
            }


            var response = await TeamsService.GetTeam(ID);

            if (response.Success)
            {
                Team = response.Data;
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


        private void EditTeam()
        {
            var encryptedId = EncryptionHelper.Encrypt(Team!.ID.ToString());
            NavManager.NavigateTo($"/add-edit-team/{encryptedId}");
        }

        private async Task ConfirmDelete()
        {
            IsDeleteDialogVisible = false;
            var response = await TeamsService.DeleteTeam(ID);

            if (response.Success)
            {
                Team = new TeamsResponse();
                await Task.Delay(100);
                await ShowToast(response.Message!, response.Success);
                NavManager.NavigateTo("/TeamsList");
            }
            else
            {
                await Task.Delay(100);
                await ShowToast(response.Message!, response.Success);
            }
        }
    }
}
