using Blazored.SessionStorage;
using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Schedule;

namespace engineering_project_front.Pages
{
    public partial class AvailabilityScheduler
    {
        private DateTime CurrentDate { get; set; } = DateTime.Today;
        private SfSchedule<AvailabilitiesResponse> ScheduleRef = default!;
        private List<AvailabilitiesResponse> dataSource { get; set; } = new();

        public long? UserID;
        public string FirstName = string.Empty;
        public string LastName = string.Empty;

        [Inject]
        private IAvailabilitiesService availabilitiesService { get; set; } = default!;

        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;

        [Inject]
        private IUsersService usersService { get; set; } = default!;

        protected async override Task OnInitializedAsync()
        {
            await GetAvailabilities();

            await GetUser();

            await base.OnInitializedAsync();
        }
        private async Task GetUser()
        {
            var token = await sessionStorage.GetItemAsStringAsync("token");

            if (token == null)
                return;

            token = token.Trim('"');

            var user = await usersService.GetUserFromToken(token);

            UserID = user.ID!;
            FirstName = user.FirstName!;
            LastName = user.LastName!;
        }

        private async Task GetAvailabilities()
        {
            var responseCurrentMonth = await availabilitiesService.GetAvailabilitiesForMonth(CurrentDate);
            var responseNextMonth = await availabilitiesService.GetAvailabilitiesForMonth(CurrentDate.AddMonths(1));

            if (responseCurrentMonth.Success)
            {
                dataSource = responseCurrentMonth.Data!.ToList();
                if (responseNextMonth.Success)
                    dataSource.AddRange(responseNextMonth.Data!.ToList());
            }
        }

        public async Task OnCellClick(CellClickEventArgs args)
        {
            args.Cancel = true;

            var startOfTheNextMonth = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1);
            var endOfTheNextMonth = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, DateTime.DaysInMonth(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month));
            var selectedDate = args.StartTime;
            

            if (selectedDate < startOfTheNextMonth || selectedDate > endOfTheNextMonth) return;
            await ScheduleRef.OpenEditorAsync(args, CurrentAction.Add); //to open the editor window on cell click
        }
        public async Task OnEventClick(EventClickArgs<AvailabilitiesResponse> args)
        {
            if (!args.Event.IsReadonly)
            {
                args.Cancel = true;
                CurrentAction action = CurrentAction.Save;
                await ScheduleRef.OpenEditorAsync(args.Event, action); //to open the editor window on event click
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

            var data = args.Data;

            data.Date = data.TimeStart.Date;//Temporary

            AvailabilitiesRequest request = new()
            {
                ID = data.ID,
                UserID = UserID.Value,
                Date = data.Date,
                TimeStart = data.TimeStart,
                TimeEnd = data.TimeEnd,
                Status = data.Status,
                Type = data.Type,
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
    }
}
