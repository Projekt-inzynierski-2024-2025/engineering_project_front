using Blazored.SessionStorage;
using engineering_project_front.Models;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class MyShifts : ComponentBase
    {
        #region Injection
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private IScheduleService ScheduleService { get; set; } = default!;
        [Inject]
        private IUsersService UsersService { get; set; } = default!;

        [Inject]
        private IWorksService WorksService { get; set; } = default!;

        [Inject]
        private ISessionStorageService SessionStorage { get; set; } = default!;

        [Inject]
        private IValidateRole validateRole { get; set; } = default!;
        #endregion


        [Parameter]
        public long? UserID { get; set; }
        private bool IsManager { get; set; } = false;
        private DateTime DataChoose = DateTime.Today;
        public string Month => DataChoose.ToString("Y");
        private UsersResponse UserToCheck { get; set; } = new UsersResponse();


        private List<ShiftWork> UserShiftWorks { get; set; } = new();
        private int WorkTimeHours = 0;
        private int WorkTimeMinutes = 0;
        private int PlannedWorkTimeHours = 0;
        private int PlannedWorkTimeMinutes = 0;

        private string TodayShift = "Brak";
        private string TomorrowShift = "Brak";

        #region ToastAndNotification
        private SfToast Toast = new();
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        #endregion



        protected override async Task OnParametersSetAsync()
        {
            if (!await validateRole.IsAuthorized("Kierownik", "Pracownik"))
                NavManager.NavigateTo("/auth-error");

            await GetUserToCheck();
            if (UserID.HasValue)

            {
                // Kierownik przegląda zmiany pracownika
                IsManager = true;
                UserShiftWorks = await GetUserShiftWork(UserID.Value, DataChoose);
                await GetTodayTomorrowShift(UserID.Value);


            }
            else
            {
                // Pracownik przegląda swoje zmiany
                IsManager = false;
                UserShiftWorks = await GetUserShiftWork(UserToCheck.ID, DataChoose);
                await GetTodayTomorrowShift(UserToCheck.ID);
            }


            var status = await ShiftStatus();
            if (status)
            {
                UserShiftWorks = new List<ShiftWork>();
                WorkTimeHours = 0;
                WorkTimeMinutes = 0;
                PlannedWorkTimeHours = 0;
                PlannedWorkTimeMinutes = 0;
                 TodayShift = "Brak";
                TomorrowShift = "Brak";
            }



            await InvokeAsync(StateHasChanged);

        }
        private async Task GetTodayTomorrowShift(long userID)
        {

            var responseUserDailySchedule = await ScheduleService.GetUsersDailySchedulesForMonth(userID, DateTime.Now);
            if (!responseUserDailySchedule.Success)
            {
                await ShowToast(responseUserDailySchedule.Message!, responseUserDailySchedule.Success);
            }
            var userPlanedShifts = responseUserDailySchedule.Data ?? new List<UsersDailySchedulesResponse>();


            // Pobranie dzisiejszej i jutrzejszej daty
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            // Znalezienie zmiany na dzisiaj
            var todayShift = userPlanedShifts.FirstOrDefault(shift => shift.TimeStart.Date == today);
            if (todayShift != null)
            {
                TodayShift = $"{todayShift.TimeStart:HH:mm} - {todayShift.TimeEnd:HH:mm}";
            }
            else
            {
                TodayShift = "Brak";
            }

            // Znalezienie zmiany na jutro
            var tomorrowShift = userPlanedShifts.FirstOrDefault(shift => shift.TimeStart.Date == tomorrow);
            if (tomorrowShift != null)
            {
                TomorrowShift = $"{tomorrowShift.TimeStart:HH:mm} - {tomorrowShift.TimeEnd:HH:mm}";
            }
            else
            {
                TomorrowShift = "Brak";
            }
        }





        private async Task<List<ShiftWork>> GetUserShiftWork(long userID, DateTime date)
        {
            // Pobranie zaplanowanych zmian
            var responseUserDailySchedule = await ScheduleService.GetUsersDailySchedulesForMonth(userID, date);
            if (!responseUserDailySchedule.Success)
            {
                await ShowToast(responseUserDailySchedule.Message!, responseUserDailySchedule.Success);
            }
            var userPlanedShifts = responseUserDailySchedule.Data ?? new List<UsersDailySchedulesResponse>();

            // Pobranie przepracowanych zmian
            var responseUserWorkedShifts = await WorksService.GetWorkForMonth(userID, date);
            if (!responseUserWorkedShifts.Success)
            {
                await ShowToast(responseUserWorkedShifts.Message!, responseUserWorkedShifts.Success);
            }
            var userWorkedShifts = responseUserWorkedShifts.Data ?? new List<WorksResponse>();

            // Resetowanie zmiennych globalnych
            WorkTimeHours = 0;
            WorkTimeMinutes = 0;
            PlannedWorkTimeHours = 0;
            PlannedWorkTimeMinutes = 0;

            // Lista wynikowa
            var userShiftWorks = new List<ShiftWork>();

            // Grupowanie danych według dni
            var groupedPlanedShifts = userPlanedShifts.GroupBy(x => x.TimeStart.Date);
            var groupedWorkedShifts = userWorkedShifts.GroupBy(x => x.Date.Date);

            // Iteracja po wszystkich dniach miesiąca
            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(date.Year, date.Month))
                                         .Select(day => new DateTime(date.Year, date.Month, day));

            foreach (var day in daysInMonth)
            {
                var plannedShift = groupedPlanedShifts.FirstOrDefault(x => x.Key == day)?.FirstOrDefault();
                var workedShift = groupedWorkedShifts.FirstOrDefault(x => x.Key == day)?.FirstOrDefault();

                // Pomijamy dzień, jeśli nie ma ani zaplanowanej, ani przepracowanej zmiany
                if (plannedShift == null && workedShift == null)
                {
                    continue;
                }

                // Obliczanie czasu pracy
                if (workedShift != null)
                {
                    var dailyWorkTime = workedShift.TimeEnd.TimeOfDay - workedShift.TimeStart.TimeOfDay - workedShift.BreakTime.TimeOfDay;
                    if (dailyWorkTime > TimeSpan.Zero)
                    {
                        WorkTimeHours += dailyWorkTime.Hours;
                        WorkTimeMinutes += dailyWorkTime.Minutes;
                    }
                }

                // Obliczanie zaplanowanego czasu pracy
                if (plannedShift != null)
                {
                    var dailyPlannedTime = plannedShift.TimeEnd - plannedShift.TimeStart;
                    if (dailyPlannedTime > TimeSpan.Zero)
                    {
                        PlannedWorkTimeHours += dailyPlannedTime.Hours;
                        PlannedWorkTimeMinutes += dailyPlannedTime.Minutes;
                    }
                }

                var shiftWork = new ShiftWork
                {
                    ID = plannedShift?.ID ?? workedShift?.UserID ?? 0,
                    Date = day,
                    TimeStartShift = plannedShift?.TimeStart ?? DateTime.MinValue,
                    TimeEndShift = plannedShift?.TimeEnd ?? DateTime.MinValue,
                    TimeStartWork = workedShift?.TimeStart ?? DateTime.MinValue,
                    TimeEndWork = workedShift?.TimeEnd ?? DateTime.MinValue,
                    TimeStartBreak = workedShift?.BreakStart ?? DateTime.MinValue,
                    TimeEndBreak = workedShift?.BreakEnd ?? DateTime.MinValue,
                };

                userShiftWorks.Add(shiftWork);
            }

            // Dodanie nadmiarowych minut jako pełnych godzin
            WorkTimeHours += WorkTimeMinutes / 60;
            WorkTimeMinutes %= 60;
            PlannedWorkTimeHours += PlannedWorkTimeMinutes / 60;
            PlannedWorkTimeMinutes %= 60;

            return userShiftWorks;
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



        private async Task<bool> ShiftStatus()
        {
            var response = await ScheduleService.GetEditStatusMonthSchedule(UserToCheck.TeamID, DataChoose.Year, DataChoose.Month);

            if (response.Success)
            {
                return response.Data;
            }
            else
            {
                await ShowToast(response.Message!, response.Success);
                return false;

            }

        }

        private async Task<List<UsersDailySchedulesResponse>> GetUsersDailySchedulesForMonth()
        {
            var token = await SessionStorage.GetItemAsStringAsync("token");

            if (string.IsNullOrEmpty(token))
                return new();

            token = token.Trim('"');

            var user = await UsersService.GetUserFromToken(token);
            var response = await ScheduleService.GetUsersDailySchedulesForMonth(user.ID, DataChoose);

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


        private async Task GetUserToCheck()
        {
            if (UserID.HasValue)
            {
                var response = await UsersService.GetUser(UserID.Value);
                if (response.Success)
                {
                    UserToCheck = response.Data!;
                }
                else
                {
                    await ShowToast(response.Message!, response.Success);

                }
            }
            else
            {
                var token = await SessionStorage.GetItemAsStringAsync("token");

                token = token.Trim('"');

                UserToCheck = await UsersService.GetUserFromToken(token);

            }
        }

        private async Task OnDateChange(ChangedEventArgs<DateTime> args)
        {
            DataChoose = args.Value;

            if (IsManager)
            {
                UserShiftWorks = await GetUserShiftWork(UserID!.Value, DataChoose);
            }
            else
            {
                UserShiftWorks = await GetUserShiftWork(UserToCheck.ID, DataChoose);
            }

            await InvokeAsync(StateHasChanged);
        }


    }
}
