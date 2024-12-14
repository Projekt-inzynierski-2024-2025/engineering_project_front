using Blazored.SessionStorage;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Calendars;

namespace engineering_project_front.Pages
{
    public partial class ShowTime
    {
        private DateTime DataChoose = DateTime.Today;
        public List<WorksResponse> GridData { get; set; } = new();
        public string Month => DataChoose.ToString("Y");
        public string MonthlyWorkTime { get; set; } = string.Empty;
        public string MonthlyBreakTime { get; set; } = string.Empty;

        private long ID;

        #region Injection
        [Inject]
        private IValidateRole validateRole { get; set; } = default!;

        [Inject]
        private NavigationManager navManager { get; set; } = default!;
        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;
        [Inject]
        private IWorksService worksService { get; set; } = default!;
        [Inject]
        private IUsersService usersService { get; set; } = default!;
        #endregion

        protected async override Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Kierownik", "Pracownik"))
                navManager.NavigateTo("/auth-error");

            await GetUser();

            await GetWork(DataChoose);

            await base.OnInitializedAsync();
        }

        private async Task GetUser()
        {
            var token = await sessionStorage.GetItemAsStringAsync("token");

            if (token == null)
                return;

            token = token.Trim('"');

            var user = await usersService.GetUserFromToken(token);

            ID = user.ID!;
        }

        private async Task OnDateChange(ChangedEventArgs<DateTime> args)
        {
            DataChoose = args.Value;

            await GetWork(DataChoose);

            await InvokeAsync(StateHasChanged);
        }

        private async Task GetWork(DateTime date)
        {
            if (ID == -1) return;
            var response = await worksService.GetWorkForMonth(ID, date);

            if (response.Success)
            {
                DateTime allWorkTime = DateTime.MinValue;
                DateTime allBreakTime = DateTime.MinValue;
                GridData = response.Data!.ToList()!;
                foreach(var workTime in response.Data!)
                {
                    allWorkTime +=(workTime.WorkTime.TimeOfDay);
                    allBreakTime += workTime.BreakTime.TimeOfDay;
                }

                MonthlyWorkTime = allWorkTime.ToString("HH:mm");
                MonthlyBreakTime = allBreakTime.ToString("HH:mm");
            }
            else
            {
                GridData = new();
                MonthlyWorkTime = "Brak danych do wyliczenia.";
                MonthlyBreakTime = "Brak danych do wyliczenia.";
            }
        }
    }
}
