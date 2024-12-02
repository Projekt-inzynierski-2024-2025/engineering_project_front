using Blazored.SessionStorage;
using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;

namespace engineering_project_front.Pages
{
    public partial class ScheduleMonth : ComponentBase
    {
        #region Injection
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private ISessionStorageService SessionStorage { get; set; } = default!;
        [Inject]
        private IScheduleService ScheduleService { get; set; } = default!;

        [Inject]
        private IUsersService UsersService { get; set; } = default!;

        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        #endregion


        private List<HoursForDayResponse> Hours { get; set; } = new();
        private long TeamID { get; set; } = 0;
        private SfDialog? TeamDialog;
        private bool IsTeamDialogVisible { get; set; } = false;
        private List<TeamsResponse> Teams { get; set; } = new();
        private long SelectedTeamID { get; set; }
        private string TeamName { get; set; } =  "Wybierz zespoł";
        private DateTime DataChoose = DateTime.Today;
        public string Month => DataChoose.ToString("Y");

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

            IsTeamDialogVisible = true;
            await InvokeAsync(StateHasChanged);

        

        }

        #region ToastAndNotification
        private async Task ShowToast(string message, bool success)
        {
            Message = message;
            Title = success ? "Sukces!" : "Błąd!";
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

        private void AddSchedule()
        {
            NavManager.NavigateTo($"/AddSchedule/{TeamID}");
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
                await LoadScheduleForTeam();
                TeamName = Teams.FirstOrDefault(t => t.ID == TeamID)?.Name;
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

        private async Task LoadScheduleForTeam()
        {
            var response = await ScheduleService.GetHoursForEachDayForMonthAsync(DataChoose.Year, DataChoose.Month, TeamID);
            if (response.Success)
            {
                Hours = response.Data;
                if (Hours.Count == 0)
                {
                    ShowToast("Brak danych", false);
                    Hours = new List<HoursForDayResponse>();
                }
            }
            else
            {
                ShowToast(response.Message, response.Success);
                Hours = new List<HoursForDayResponse>();
            }
        }


        private async Task OnDateChange(ChangedEventArgs<DateTime> args)
        {
            DataChoose = args.Value;

            await LoadScheduleForTeam();

            await InvokeAsync(StateHasChanged);
        }
        private void OnContextMenuClick(ContextMenuClickEventArgs<HoursForDayResponse> args)
        {
            if (args == null)
                return;

            switch (args.Item.Id)
            {
                case "seeDetails":
                    NavManager.NavigateTo($"/Schedule/{args.RowInfo.RowData.DailyScheduleID}");
                    break;
                default:
                    break;
            }
        }
    }
}
