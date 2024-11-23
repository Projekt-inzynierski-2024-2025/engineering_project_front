using Blazored.SessionStorage;
using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;

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

        #region ToastAndNotification
        private SfToast? Toast;
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            TeamID = await GetTeamID();

            if (TeamID == 0)
            {
                ShowToast("Nie znaleziono zespołu", false);
                return;
            }

            var response = await ScheduleService.GetHoursForEachDayForMonthAsync(DateTime.Now.Year, DateTime.Now.Month, TeamID);
            if (response.Success)
            {
                Hours = response.Data;
                if (Hours.Count == 0)
                {
                    ShowToast("Brak danych", false);
                }
            }
            else
            {
                ShowToast(response.Message, response.Success);
            }


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

        private async Task<long> GetTeamID()
        {
            var token = await SessionStorage.GetItemAsStringAsync("token");

            if (string.IsNullOrEmpty(token))
                return 0;

            token = token.Trim('"');

            var user = await UsersService.GetUserFromToken(token);
            var response = await TeamsService.GetTeamIDForManager(user.Email);

            if (response.Success)
            {
                return response.Data;
            }
            else
            {
                ShowToast(response.Message, response.Success);
                return 0;
            }
        }

        private void AddSchedule()
        {
            NavManager.NavigateTo($"/AddSchedule/{TeamID}");
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
