using Blazored.SessionStorage;
using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Schedule;

namespace engineering_project_front.Pages
{
    public partial class AvailabilityScheduler
    {
        private DateTime CurrentDate { get; set; } = DateTime.Today;
        private SfSchedule<AvailabilitiesResponse> ScheduleRef = default!;
        private View CurrentView { get; set; } = View.Week;
        private List<AvailabilitiesResponse> dataSource { get; set; } = new();

        private DateTime minDate = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1);
        private DateTime maxDate = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, DateTime.DaysInMonth(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month));
        private DateTime minTime = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1, 5, 0, 0);
        private DateTime maxTime = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, DateTime.DaysInMonth(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month), 23, 0, 0);

        private bool isEditing = false;

        public long? UserID;
        public string FirstName = string.Empty;
        public string LastName = string.Empty;

        #region Injections
        [Inject]
        private IValidateRole validateRole { get; set; } = default!;

        [Inject]
        private IAvailabilitiesService availabilitiesService { get; set; } = default!;

        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;

        [Inject]
        private IUsersService usersService { get; set; } = default!;

        [Inject]
        private NavigationManager navManager { get; set; } = default!;
        #endregion

        #region Toast
        private SfToast? Toast;
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion

        protected async override Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Kierownik", "Pracownik"))
                navManager.NavigateTo("/auth-error");

            await GetUser();

            await GetAvailabilities();

            await base.OnInitializedAsync();
        }
        private async Task GetUser()
        {
            var token = await sessionStorage.GetItemAsStringAsync("token");

            if (token == null)
            {
                ShowToast("Błąd", "Nie można pobrać danych zalogowanej osoby. Nie będzie można podejrzeć dyspozycyjności, dodać nowych ani edytować.");
                return;
            }

            token = token.Trim('"');

            var user = await usersService.GetUserFromToken(token);

            UserID = user.ID!;
            FirstName = user.FirstName!;
            LastName = user.LastName!;
        }

        private async Task GetAvailabilities()
        {
            var responseCurrentMonth = await availabilitiesService.GetAvailabilitiesForMonth(UserID!.Value, CurrentDate);
            var responseNextMonth = await availabilitiesService.GetAvailabilitiesForMonth(UserID!.Value, CurrentDate.AddMonths(1));

            if (responseCurrentMonth.Success)
            {
                dataSource = responseCurrentMonth.Data!.ToList();

                if (responseNextMonth.Success)
                    dataSource.AddRange(responseNextMonth.Data!.ToList());

                dataSource.ForEach(x =>
                {
                    if (x.UserID == UserID)
                        x.CategoryColor = "#008000";
                });
            }
        }

        public async Task OnCellClick(CellClickEventArgs args)
        {
            args.Cancel = true;

            var startOfTheNextMonth = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1);
            var endOfTheNextMonth = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, DateTime.DaysInMonth(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month));
            var aWeekBeforeEndOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)).AddDays(-7);
            var selectedDate = args.StartTime;
            isEditing = false;

            if (selectedDate < startOfTheNextMonth || selectedDate > endOfTheNextMonth || DateTime.Today >= aWeekBeforeEndOfMonth) return;
            await ScheduleRef.OpenEditorAsync(args, CurrentAction.Add); //to open the editor window on cell click
        }
        public async Task OnEventClick(EventClickArgs<AvailabilitiesResponse> args)
        {
            if (!args.Event.IsReadonly)
            {
                args.Cancel = true;
                CurrentAction action = CurrentAction.Save;
                isEditing = true;
                await ScheduleRef.OpenEditorAsync(args.Event, action); //to open the editor window on event click
            }
        }
        public void OnEventRendered(EventRenderedArgs<AvailabilitiesResponse> args)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            if (CurrentView == View.Agenda && CurrentView == View.MonthAgenda)
            {
                attributes.Add("style", "border-left-color: " + args.Data.CategoryColor);
            }
            else
            {
                attributes.Add("style", "background: " + args.Data.CategoryColor);
            }
            args.Attributes = attributes;
        }
        public void OnActionBegin(ActionEventArgs<AvailabilitiesResponse> args)
        {

            if (args.ActionType == ActionType.EventCreate)
            {
                bool containsSameDate = args.AddedRecords.Any(arg => dataSource.Any(ds => ds.Date == arg.Date));

                if (containsSameDate)
                {
                    args.AddedRecords = null;
                    ShowToast("Błąd", "Dyspozycyjność istnieje dla tego dnia.");
                }
                else if(!isTimeValid(args.AddedRecords.First().TimeStart, args.AddedRecords.First().TimeEnd))
                {
                    args.AddedRecords = null;
                    ShowToast("Błąd", "Podano nieprawidłowy czas.");
                }

                return;
            }


            if (args.ActionType == ActionType.EventChange)
            {
                if (!isTimeValid(args.ChangedRecords.First().TimeStart, args.ChangedRecords.First().TimeEnd))
                {
                    args.ChangedRecords = null;
                    ShowToast("Błąd", "Podano nieprawidłowy czas.");
                }

                return;
            }
        }

        public void OnPopupClose(PopupCloseEventArgs<AvailabilitiesResponse> args)
        {
            if (args.Cancel) return;

            if (UserID == null) return;

            args.Data.TimeStart = args.Data.Date.Date + args.Data.TimeStart.TimeOfDay;
            args.Data.TimeEnd = args.Data.Date.Date + args.Data.TimeEnd.TimeOfDay;
            args.Data.FirstName = FirstName;
            args.Data.LastName = LastName;
            args.Data.CategoryColor = "#008000";

            var data = args.Data;

            AvailabilitiesRequest request = new()
            {
                ID = data.ID,
                UserID = UserID.Value,
                Date = data.Date,
                TimeStart = data.TimeStart,
                TimeEnd = data.TimeEnd,
                Status = 0,
                Type = 1,
            };

            switch (args.Type)
            {
                case PopupType.Editor:
                    if (args.CurrentAction == CurrentAction.Add) //Dodawanie
                    {
                        availabilitiesService.CreateAvailabilities(request);
                        break;
                    }
                    else if (args.CurrentAction == CurrentAction.Save) //Edycja
                    {
                        availabilitiesService.UpdateAvailability(request);
                        break;
                    }
                    break;
                case PopupType.DeleteAlert: //Usuwanie
                    availabilitiesService.RemoveAvailability(request);
                    break;
            }
        }

        private void ShowToast(string title, string message)
        {
            Title = title;
            Message = message;
            InvokeAsync(StateHasChanged);
            Toast?.ShowAsync();
        }

        private bool isTimeValid(DateTime timeStart, DateTime timeEnd)
        {
            return timeStart < timeEnd;
        }
    }
}
