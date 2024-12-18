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
    public partial class MyEmployees : ComponentBase
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

        [Inject]
        private IValidateRole validateRole { get; set; } = default!;
        #endregion

        private List<HoursForUserForMonthResponse> HoursForUsers { get; set; } = new();
        private long TeamID { get; set; } = 0;
        private string TeamName { get; set; } = "";
        private SfDialog TeamDialog = default!;
        private bool IsTeamDialogVisible { get; set; } = false;
        private List<TeamsResponse> Teams { get; set; } = new();
        private long SelectedTeamID { get; set; }
        private bool IsEmployeeWorkDialogVisible { get; set; } = false;
        private long SelectedUserID { get; set; }
        private DateTime SelectedWorkDate { get; set; } = DateTime.Today;
        private long UserID { get; set; } = 0;

        DateTime minDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);

        DateTime maxDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 30);

        private DateTime DataChoose = DateTime.Today;

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
            await GetUserToCheck();

            if (Teams is null)
            {
                await ShowToast("Nie znaleziono zespołów", false);
                return;
            }

            if (Teams.Count == 1)
            {
                TeamID = Teams[0].ID;
                TeamName = Teams[0].Name;
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
                TeamName = Teams.Find(team => team.ID == SelectedTeamID)!.Name;
                IsTeamDialogVisible = false;
                await LoadEmployesForTeam();
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

                
                var filteredTeams = response.Data!.Where(team => teamsIDs.Contains(team.ID)).ToList();
                return filteredTeams;
            }
            else
            {
                await ShowToast(response.Message!, false);
                return new List<TeamsResponse>();
            }
        }

        private async Task LoadEmployesForTeam()
        {
            var response = await ScheduleService.GetUsersHoursForMonth(DataChoose.Year, DataChoose.Month, TeamID);
            if (response.Success)
            {
                HoursForUsers = response.Data!;
            }
            else
            {
                await ShowToast(response.Message!, response.Success);
                HoursForUsers = new List<HoursForUserForMonthResponse>();
            }
        }



        private async void OnContextMenuClick(ContextMenuClickEventArgs<HoursForUserForMonthResponse> args)
        {
            if (args == null)
                return;

            switch (args.Item.Id)
            {

                case "seeDetails":
                    if (args.RowInfo.RowData.userID != UserID)
                    {
                        NavManager.NavigateTo($"/myShifts/{args.RowInfo.RowData.userID}");
                    }
                    else
                    {
                       await ShowToast("Przejdź do zakładki moje zmiany", false);
                    }
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
                    await ShowToast("Umożliwiono zmiane czasu pracy", true);
                }
                else
                {
                    await ShowToast(response.Message!, response.Success);
                }
                IsEmployeeWorkDialogVisible = false;
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await ShowToast("Proszę wybrać pracownika", false);
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
                if (workHours > 168 || workHours == 0)
                {

                    args.Cell.AddClass(["highlight-red"]);
                }
            }
        }

        private async Task GetUserToCheck()
        {

            var token = await SessionStorage.GetItemAsStringAsync("token");

            token = token.Trim('"');

            var user = await UsersService.GetUserFromToken(token);
            UserID = user.ID;


        }
    }
}
