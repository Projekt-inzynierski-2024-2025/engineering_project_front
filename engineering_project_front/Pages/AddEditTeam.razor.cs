﻿using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class AddEditTeam : ComponentBase
    {

        #region Injects
        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        [Inject]
        private IUsersService UsersService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private IValidateRole validateRole { get; set; } = default!;
        #endregion


        [Parameter]
        public string? TeamId { get; set; }

        public long? TeamID { get; set; }

        private TeamRequest Team { get; set; } = new TeamRequest();
        private bool IsEditing => !string.IsNullOrEmpty(TeamId);
        private List<UsersResponse> Managers { get; set; } = new List<UsersResponse>();

        #region Toast
        private SfToast Toast = new();
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Administrator"))
                NavManager.NavigateTo("/auth-error");

            var responseManagers = await UsersService.GetManagers();
            if (responseManagers.Success)
            {
                Managers = responseManagers.Data!;

            }
            else
            {
                await Task.Delay(100);
                await ShowToast(responseManagers.Message!, responseManagers.Success);
            }
            if (IsEditing)
            {
                try
                {
                    TeamID = long.Parse(EncryptionHelper.Decrypt(TeamId));
                }
                catch
                {
                    await Task.Delay(100);
                    await ShowToast("Niepoprawny identyfikator użytkownika.", false);
                    NavManager.NavigateTo("/error"); // Przekierowanie na stronę błędu
                    return;
                }
                var response = await TeamsService.GetTeam((long)TeamID!) ?? new OperationResponse<TeamsResponse>();
                if (response.Success)
                {
                    var team = response.Data;
                    if (team != null)
                    {
                        MapResponseToRequest(team);
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
        private void MapResponseToRequest(TeamsResponse res)
        {

            Team.ID = res.ID;
            Team.Name = res.Name;
            Team.ManagerID = res.ManagerID;


        }

        #endregion



        private async Task HandleValidSubmit()
        {
            if (IsEditing)
            {
                var response = await TeamsService.EditTeam(Team);
                if (response.Success)
                {
                    await Task.Delay(100);
                    await ShowToast(response.Message!, response.Success);
                    await Task.Delay(2000);
                    var encryptedId = EncryptionHelper.Encrypt(Team.ID.ToString());
                    NavManager.NavigateTo($"/TeamDetails/{encryptedId}");
                }
                else
                {
                    await Task.Delay(100);
                    await ShowToast(response.Message!, response.Success);
                }
            }
            else
            {
                var response = await TeamsService.AddTeam(Team);
                if (response.Success)
                {
                    await Task.Delay(100);
                    await ShowToast(response.Message!, response.Success);
                    await Task.Delay(2000);
                    NavManager.NavigateTo("/TeamsList");
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
            if(IsEditing)
            {
                var encryptedId = EncryptionHelper.Encrypt(Team.ID.ToString());
                NavManager.NavigateTo($"/TeamDetails/{encryptedId}");
            }
            else
            {
                NavManager.NavigateTo("/TeamsList");
            }
           
        }
    }
}
