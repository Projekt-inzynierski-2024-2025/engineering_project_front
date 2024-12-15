using Blazored.SessionStorage;
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

        [Inject]
        private IValidateRole validateRole { get; set; } = default!;
        #endregion

        private List<HoursForDayResponse> Hours { get; set; } = new();
        private long TeamID { get; set; } = 0;
        private SfDialog TeamDialog = default!;
        private bool IsTeamDialogVisible { get; set; } = false;
        private List<TeamsResponse> Teams { get; set; } = new();
        private long SelectedTeamID { get; set; }
        private string TeamName { get; set; } =  "Wybierz zespoł";
        private DateTime DataChoose = DateTime.Today;
        public string Month => DataChoose.ToString("Y");
        private bool EditStatus { get; set; } = false;

        #region ToastAndNotification
        private SfToast Toast = new();
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Kierownik"))
                NavManager.NavigateTo("/auth-error");

            Teams = await GetTeams();
           

            if (Teams is null)
            {
                await ShowToast("Nie znaleziono zespołów", false);
                return;
            }

            if (Teams.Count == 1)
            {
                TeamID = Teams[0].ID;
                await LoadScheduleForTeam();
                await GetScheduleStatus();
                TeamName = Teams.First(t => t.ID == TeamID).Name;
                await InvokeAsync(StateHasChanged);
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
            Title = success ? "Sukces!" : "Błąd!";
            await InvokeAsync(StateHasChanged);
            await Toast.ShowAsync();
        }
        #endregion

        private async Task<List<long>> GetTeamsID()
        {
            var token = await SessionStorage.GetItemAsStringAsync("token");

            if (string.IsNullOrEmpty(token))
                return new();

            token = token.Trim('"');

            var user = await UsersService.GetUserFromToken(token);
            var response = await TeamsService.GetTeamsIDForManager(user.Email!);

            if (response.Success)
            {
                return response.Data!;
            }
            else
            {
                await ShowToast(response.Message!, response.Success);
                return new();
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
                TeamName = Teams.First(t => t.ID == TeamID).Name;
                await GetScheduleStatus();
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await ShowToast("Proszę wybrać zespół", false);
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
                    await ShowToast("Nie znaleziono zespołów dla tego użytkownika", false);
                    return new List<TeamsResponse>();
                }

                // Przefiltruj zespoły na podstawie ID
                var filteredTeams = response.Data!.Where(team => teamsIDs.Contains(team.ID)).ToList();
                return filteredTeams;
            }
            else
            {
                await ShowToast(response.Message!, false);
                return new List<TeamsResponse>();
            }
        }

        private async Task LoadScheduleForTeam()
        {
            var response = await ScheduleService.GetHoursForEachDayForMonthAsync(DataChoose.Year, DataChoose.Month, TeamID);
            if (response.Success)
            {
                Hours = response.Data!;
                if (Hours.Count == 0)
                {
                    await ShowToast("Brak danych", false);
                    Hours = new List<HoursForDayResponse>();
                }
            }
            else
            {
                await ShowToast(response.Message!, response.Success);
                Hours = new List<HoursForDayResponse>();
            }
        }

        private async Task GetScheduleStatus()
        {
            var response = await ScheduleService.GetEditStatusMonthSchedule(TeamID, DataChoose.Year, DataChoose.Month);
            if (response.Success)
            {
                EditStatus = response.Data;
            }
            else
            {
                await ShowToast(response.Message!, response.Success);
            }
            
        }

        private async Task ChangeScheduleStatus()
        {
            var response = await ScheduleService.ChangeEditStatusMonthSchedule(TeamID, DataChoose.Year, DataChoose.Month);
            if (response.Success)
            {
                EditStatus = !EditStatus;
                await ShowToast("Zmieniono status edycji", true);
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await ShowToast(response.Message!, response.Success);
            }
        }


        private async Task OnDateChange(ChangedEventArgs<DateTime> args)
        {
            DataChoose = args.Value;

            await LoadScheduleForTeam();
            await GetScheduleStatus();
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


        private void OnQueryCellInfo(QueryCellInfoEventArgs<HoursForDayResponse> args)
        {
            var ToDoHours = Convert.ToDouble(args.Data.ToDoHours);
            var workHours = Convert.ToDouble(args.Data.WorkHours);

            var result = workHours - ToDoHours;

            if (result < 0 || result > 15)
            {


                if (args.Column.Field == nameof(HoursForDayResponse.ToDoHours))
                {
                    args.Cell.AddClass(["highlight-red"]);
                }

                if (args.Column.Field == nameof(HoursForDayResponse.WorkHours))
                {
                        args.Cell.AddClass(["highlight-red"]);               
                }

            }

            if (0 < result && result < 15)
            {
                if (args.Column.Field == nameof(HoursForDayResponse.ToDoHours))
                {
                    args.Cell.AddClass(["highlight-green"]);
                }

                if (args.Column.Field == nameof(HoursForDayResponse.WorkHours))
                {
                    args.Cell.AddClass(["highlight-green"]);
                }

            }
        }
    }
}
