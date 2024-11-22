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

        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;
        [Inject]
        private IWorksService worksService { get; set; } = default!;
        [Inject]
        private IUsersService usersService { get; set; } = default!;

        protected async override Task OnInitializedAsync()
        {
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
                TimeSpan allWorkTime = TimeSpan.Zero;
                TimeSpan allBreakTime = TimeSpan.Zero;
                GridData = response.Data!.ToList()!;
                foreach(var workTime in response.Data!)
                {
                    allWorkTime += workTime.WorkTime;
                    allBreakTime += workTime.BreakTime;
                }

                MonthlyWorkTime = allWorkTime.ToString("hh':'mm':'ss");
                MonthlyBreakTime = allBreakTime.ToString("hh':'mm':'ss");
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
