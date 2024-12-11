using Blazored.SessionStorage;

using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;

using Microsoft.AspNetCore.Components;

namespace engineering_project_front.Pages
{
    public partial class Home
    {
        [Inject]
        private IValidateRole validateRole { get; set; } = default!;

        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;

        [Inject]
        private IUsersService usersService { get; set; } = default!;

        [Inject]
        private IWorksService worksService { get; set; } = default!;

        private Timer timer = default!;

        private long ID = -1;
        private string firstName = "<FIRST_NAME_PH>";
        private string lastName = "<LAST_NAME_PH>";
        private string role = string.Empty;

        private WorksResponse work = new();

        TimeZoneInfo timeZoneInfo { get; set; } = TimeZoneInfo.Local;
        private int getHourOffset(DateTime date) => timeZoneInfo.GetUtcOffset(date).Hours;
        private DateTime DateTimeNowOffsetted => DateTime.Now.AddHours(getHourOffset(DateTime.Now));

        private bool workStarted => work.TimeStart != DateTime.MinValue;
        private bool workEnded => work.TimeEnd != DateTime.MinValue;
        private bool breakStarted => work.BreakStart != DateTime.MinValue;
        private bool breakEnded => work.BreakEnd != DateTime.MinValue;

        private bool workStartDisabled => workStarted || ID == -1;
        private bool workEndDisabled => !workStarted || (breakStarted && !breakEnded) || workEnded;
        private bool breakStartDisabled => !(workStarted && !workEnded && !breakStarted);
        private bool breakEndDisabled => !(workStarted && !workEnded && breakStarted && !breakEnded);

        private string? workTime;
        private string? breakTime;

        protected async override Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Administrator", "Kierownik", "Pracownik"))
                navManager.NavigateTo("/auth-error");

            role = await sessionStorage.GetItemAsStringAsync("role");

            await GetUser();

            work.UserID = ID;

            await GetWork();


            if (workStarted && !workEnded)
            {
                if (!breakStarted && !breakEnded)
                    breakTime = "Jeszcze nie wziąłeś sobie przerwy";

                if (!breakStarted || breakEnded)
                    SetTimer(TickWork);
                else if (breakStarted && !breakEnded)
                {
                    SetTimer(TickBreak);
                    workTime = (work.BreakStart.TimeOfDay - work.TimeStart.TimeOfDay).ToString("hh':'mm':'ss");
                }

                if (breakEnded)
                    breakTime = (work.BreakEnd.TimeOfDay - work.BreakStart.TimeOfDay).ToString("hh':'mm':'ss");
            }
            else if (workEnded)
            {
                workTime = (work.TimeEnd.TimeOfDay - work.TimeStart.TimeOfDay - (work.BreakEnd.TimeOfDay - work.BreakStart.TimeOfDay)).ToString("hh':'mm':'ss");

                if (!breakStarted)
                    breakTime = "Dzisiaj nie wziąłeś przerwy";
                else
                    breakTime = (work.BreakEnd.TimeOfDay - work.BreakStart.TimeOfDay).ToString("hh':'mm':'ss");
            }
            else
            {
                workTime = "Twoja praca jeszcze się nie rozpoczęła";
                breakTime = "Nie możesz rozpocząć przerwy, dopóki nie rozpoczniesz pracy";
            }


            await base.OnInitializedAsync();
        }

        #region Timer
        private void SetTimer(TimerCallback tick)
        {
            timer = new(tick, null, 0, 1000);
        }
        private void TickWork(object? _)
        {
            workTime = (DateTime.Now.TimeOfDay - work.TimeStart.TimeOfDay - (work.BreakEnd.TimeOfDay - work.BreakStart.TimeOfDay)).ToString("hh':'mm':'ss");
            InvokeAsync(StateHasChanged);
        }
        private void TickBreak(object? _)
        {
            breakTime = (DateTime.Now.TimeOfDay - work.BreakStart.TimeOfDay).ToString("hh':'mm':'ss");
            InvokeAsync(StateHasChanged);
        }
        public void Dispose()
        {
            if (timer != null)
                timer.Dispose();
        }
        #endregion

        #region OnClick
        private async void OnWorkStartClick()
        {
            WorksRequest request = new()
            {
                UserID = ID,
                TimeStart = DateTimeNowOffsetted
            };

            var result = await worksService.StartWork(request);

            if (result.Success == true)
            {

                work.TimeStart = DateTime.Now;
                SetTimer(TickWork);
            }
        }

        private async void OnWorkEndClick()
        {
            WorksRequest request = new()
            {
                UserID = ID,
                TimeEnd = DateTimeNowOffsetted,
            };

            var result = await worksService.EndWork(request);

            if (result.Success == true)
            {
                work.TimeEnd = DateTime.Now;
                await timer.DisposeAsync();
                workTime = (work.TimeEnd.TimeOfDay - work.TimeStart.TimeOfDay - (work.BreakEnd.TimeOfDay - work.BreakStart.TimeOfDay)).ToString("hh':'mm':'ss");
                await InvokeAsync(StateHasChanged);
            }
        }

        private async void OnBreakStartClick()
        {
            WorksRequest request = new()
            {
                UserID = ID,
                BreakStart = DateTimeNowOffsetted,
            };

            var result = await worksService.StartBreak(request);

            if (result.Success == true)
            {
                work.BreakStart = DateTime.Now;
                await timer.DisposeAsync();
                workTime = (work.BreakStart.TimeOfDay - work.TimeStart.TimeOfDay).ToString("hh':'mm':'ss");
                SetTimer(TickBreak);
            }
        }

        private async void OnBreakEndClick()
        {
            WorksRequest request = new()
            {
                UserID = ID,
                BreakEnd = DateTimeNowOffsetted,
            };

            var result = await worksService.EndBreak(request);

            if (result.Success == true)
            {
                work.BreakEnd = DateTime.Now;
                await timer.DisposeAsync();
                breakTime = (work.BreakEnd.TimeOfDay - work.BreakStart.TimeOfDay).ToString("hh':'mm':'ss");
                SetTimer(TickWork);
            }
        }
        #endregion

        private async Task GetUser()
        {
            var token = await sessionStorage.GetItemAsStringAsync("token");

            if (token == null)
                return;

            token = token.Trim('"');

            var user = await usersService.GetUserFromToken(token);

            ID = user.ID!;
            firstName = user.FirstName!;
            lastName = user.LastName!;
        }

        private async Task GetWork()
        {
            if (ID == -1) return;

            var response = await worksService.GetWorkForDay(ID, DateTimeNowOffsetted);

            if (response.Success)
            {
                work = response.Data!;
            }
        }
    }
}
