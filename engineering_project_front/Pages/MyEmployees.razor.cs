using Blazored.SessionStorage;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using System.Data;

namespace engineering_project_front.Pages
{
    public partial class MyEmployees:ComponentBase
    {
        #region Injection
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private IScheduleService ScheduleService { get; set; } = default!;
        [Inject]
        private IUsersService UsersService { get; set; } = default!;
        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        [Inject]
        private IWorksService WorksService { get; set; } = default!;
        [Inject]
        private ISessionStorageService SessionStorage { get; set; } = default!;


        #endregion

        private List<HoursForUserForMonthResponse> HoursForUsers { get; set; } = new();
        private long TeamID { get; set; } = 0;
        private SfDialog? TeamDialog;
        private bool IsTeamDialogVisible { get; set; } = false;
        private List<TeamsResponse> Teams { get; set; } = new();
        private long SelectedTeamID { get; set; }
        private bool IsEmployeeWorkDialogVisible { get; set; } = false;
        private long  SelectedUserID { get; set; } 
        private DateTime SelectedWorkDate { get; set; }

        DateTime minDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);

        DateTime maxDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 30);

        private DateTime DataChoose = DateTime.Today;

        #region ToastAndNotification
        private SfToast? Toast;
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {

            Teams = await GetTeams();

            if (Teams is null)
            {
                ShowToast("Nie znaleziono zespołów", false);
                return;
            }

            if(Teams.Count == 1)
            {
                TeamID = Teams[0].ID;
                await LoadEmployesForTeam();
            }
            else
            {
                IsTeamDialogVisible = true;
                await InvokeAsync(StateHasChanged);
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
            await Toast?.ShowAsync();
        }

        #endregion


        private async Task<List<long>> GetTeamsID()
        {
            var token = await SessionStorage.GetItemAsStringAsync("token");

            if (string.IsNullOrEmpty(token))
                return null;

            token = token.Trim('"');

            var user = await UsersService.GetUserFromToken(token);
            var response = await TeamsService.GetTeamsIDForManager(user.Email);

            if (response.Success)
            {
                return response.Data;
            }
            else
            {
                ShowToast(response.Message, response.Success);
                return null;
            }
        }

        private async Task OpenTeamDialog()
        {
            Teams = await GetTeams();
            IsTeamDialogVisible = true;
            await InvokeAsync(StateHasChanged);
        }


        private async Task ConfirmTeamSelection()
        {
            if (SelectedTeamID > 0)
            {
                TeamID = SelectedTeamID;
                IsTeamDialogVisible = false;
               await LoadEmployesForTeam();
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                ShowToast("Proszę wybrać zespół", false);
            }
        }

        private void CloseTeamDialog()
        {
            IsTeamDialogVisible = false;
            StateHasChanged();
        }

        private async Task<List<TeamsResponse>> GetTeams()
        {
            var response = await TeamsService.GetTeamsAsync();
            if (response.Success)
            {
                var teamsIDs = await GetTeamsID();
                if (teamsIDs == null || teamsIDs.Count == 0)
                {
                    ShowToast("Nie znaleziono zespołów dla tego użytkownika", false);
                    return new List<TeamsResponse>();
                }

                // Przefiltruj zespoły na podstawie ID
                var filteredTeams = response.Data.Where(team => teamsIDs.Contains(team.ID)).ToList();
                return filteredTeams;
            }
            else
            {
                ShowToast(response.Message, false);
                return new List<TeamsResponse>();
            }
        }

        private async Task LoadEmployesForTeam()
        {
            var response = await ScheduleService.GetUsersHoursForMonth(DataChoose.Year, DataChoose.Month, TeamID);
            if (response.Success)
            {
                HoursForUsers = response.Data;
            }
            else
            {
                ShowToast(response.Message, response.Success);
                HoursForUsers = new List<HoursForUserForMonthResponse>();
            }
        }

     

        private void OnContextMenuClick(ContextMenuClickEventArgs<HoursForUserForMonthResponse> args)
        {
            if (args == null)
                return;

            switch (args.Item.Id)
            {
                case "seeDetails":
                    NavManager.NavigateTo($"/myShifts/{args.RowInfo.RowData.userID}");
                    break;
                default:
                    break;
            }
        }



        #region EmployeeWorkDialog

        private async Task OpenEmployeeWorkDialog()
        {
            IsEmployeeWorkDialogVisible = true;
            await InvokeAsync(StateHasChanged);
        }


        private async Task ConfirmEmployeeWorkSelection()
        {
            if (SelectedUserID > 0)
            {
                var response = await WorksService.ChangeWorkStatus(SelectedUserID, SelectedWorkDate);
                if (response.Success)
                {
                    ShowToast("Umożliwiono zmiane czasu pracy", true);
                }
                else
                {
                    ShowToast(response.Message, response.Success);
                }
                IsEmployeeWorkDialogVisible = false;
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                ShowToast("Proszę wybrać pracownika", false);
            }
        }

        private void CloseEmployeeWorkDialog()
        {
            IsEmployeeWorkDialogVisible = false;
            StateHasChanged();
        }




        #endregion

        private async Task OnDateChange(ChangedEventArgs<DateTime> args)
        {
            DataChoose = args.Value;

            await LoadEmployesForTeam();
            await InvokeAsync(StateHasChanged);
        }

        private void OnQueryCellInfo(QueryCellInfoEventArgs<HoursForUserForMonthResponse> args)
        {
            if (args.Column.Field == nameof(HoursForUserForMonthResponse.workHoursForMonth))
            {
                var workHours = Convert.ToDouble(args.Data.workHoursForMonth);
                if (workHours > 5)
                {
                    args.Cell.AddClass(new string[] { "highlight-red" });
                }
            }
        }
    }
}
