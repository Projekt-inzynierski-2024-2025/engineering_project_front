using Blazored.SessionStorage;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Calendars;

namespace engineering_project_front.Pages
{

    public class WorksResponseViewModel
    {
        public long UserID { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly TimeStart { get; set; }
        public TimeOnly TimeEnd { get; set; }
        public TimeOnly BreakStart { get; set; }
        public TimeOnly BreakEnd { get; set; }
        public TimeOnly BreakTime { get; set; }
        public TimeOnly WorkTime { get; set; }

    }
        public partial class ShowTime
    {
        private DateTime DataChoose = DateTime.Today;
        public List<WorksResponse> GridData { get; set; } = new();
        public List<WorksResponseViewModel> GridDataView { get; set; } = new();
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
                int allHoursWorkTime = 0, allMinutesWorkTime = 0;
                int allHoursBreakTime = 0, allMinutesBreakTime = 0;

                GridData = response.Data!.Where(w => w.TimeEnd != DateTime.MinValue).ToList()!;
                GridDataView = ConvertToViewModel(GridData);
                foreach (var workTime in GridData)
                {
                    allHoursWorkTime += workTime.WorkTime.TimeOfDay.Hours;
                    allMinutesWorkTime += workTime.WorkTime.TimeOfDay.Minutes;

                    allHoursBreakTime += workTime.BreakTime.TimeOfDay.Hours;
                    allMinutesBreakTime += workTime.BreakTime.TimeOfDay.Minutes;
                }

                allMinutesWorkTime += allHoursWorkTime * 60;
                allMinutesBreakTime += allHoursBreakTime * 60;

                allHoursBreakTime = allMinutesBreakTime / 60;
                allMinutesBreakTime = allMinutesBreakTime % 60;

                allHoursWorkTime = allMinutesWorkTime / 60; 
                allMinutesWorkTime = allMinutesWorkTime % 60;

                MonthlyWorkTime = $"{allHoursWorkTime} h {allMinutesWorkTime:D2} min";
                MonthlyBreakTime = $"{allHoursBreakTime} h {allMinutesBreakTime:D2} min";
            }
            else
            {
                GridData = new();
                GridDataView = new();
                MonthlyWorkTime = "Brak danych do wyliczenia.";
                MonthlyBreakTime = "Brak danych do wyliczenia.";
            }
        }



        List<WorksResponseViewModel> ConvertToViewModel(List<WorksResponse> gridData)
        {
            return gridData.Select(wr => new WorksResponseViewModel
            {
                UserID = wr.UserID,
                Date = wr.Date,
                TimeStart = TimeOnly.FromDateTime(wr.TimeStart),
                TimeEnd = TimeOnly.FromDateTime(wr.TimeEnd),
                BreakStart = TimeOnly.FromDateTime(wr.BreakStart),
                BreakEnd = TimeOnly.FromDateTime(wr.BreakEnd),
                BreakTime = TimeOnly.FromDateTime(wr.BreakTime),
                WorkTime = TimeOnly.FromDateTime(wr.WorkTime)
            }).ToList();
        }



    }
}
