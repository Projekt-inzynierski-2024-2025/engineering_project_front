using engineering_project_front.Models;
using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Schedule;

namespace engineering_project_front.Pages
{
    public partial class Schedule : ComponentBase
    {
        #region Injects
        [Inject]
        private IScheduleService ScheduleService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        [Inject]
        private IUsersService UsersService { get; set; } = default!;
        [Inject]
        private IWorksService WorksService { get; set; } = default!;
        [Inject]
        private IAvailabilitiesService AvailabilitiesService { get; set; } = default!;
        [Inject]
        private IValidateRole validateRole { get; set; } = default!;

        #endregion

        #region parameters
        [Parameter]
        public required string ParamID { get; set; }

        private long ID => long.Parse(EncryptionHelper.Decrypt(ParamID));

        #endregion


        #region ScheduleView
        SfSchedule<ShiftAppointment> ScheduleRef = default!;
        public int IntervalInMinutes { get; set; } = 60;
        public string StartTime { get; set; } = "05:00";
        public string EndTime { get; set; } = "23:00";
        public string StartTimeView { get; set; } = "05:00";
        public string EndTimeView { get; set; } = "23:00";
        public bool IsLayoutChanged = false;
        public string[] Resources { get; set; } = { "Employees" };


        public List<EmployeeResource> EmployeeResources { get; set; } = new List<EmployeeResource>();
        public List<ShiftAppointment> ShiftAppointments { get; set; } = new List<ShiftAppointment>();
        private List<UsersResponse> Employees { get; set; } = new List<UsersResponse>();
        private List<UsersDailySchedulesResponse> UsersDailySchedulesResponses { get; set; } = new List<UsersDailySchedulesResponse>();
        private List<AvailabilitiesResponse> Availabilities { get; set; } = new List<AvailabilitiesResponse>();
        private IEnumerable<WorksResponse> Works { get; set; } = new List<WorksResponse>();

        private UsersDailySchedulesResponse ShiftToEdit { get; set; } = new UsersDailySchedulesResponse();

        private DateTime CurrentDate { get; set; } = DateTime.Today;

        private DailySchedulesResponse DailySchedule { get; set; } = new DailySchedulesResponse();

        #endregion


        #region EditAddOneShift
        private bool isCell;
        private bool isEvent;
        private bool isResource;
        private CellClickEventArgs CellData { get; set; } = default!;
        private ShiftAppointment EventData { get; set; } = default!;
        private ElementInfo<ShiftAppointment> ElementData { get; set; } = default!;
        private bool IsShiftDialogVisible = false;

        public DateTime MinVal { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 05, 00, 00);
        public DateTime MaxVal { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 23, 00, 00);

        #endregion


        private Double TotalHours { get; set; } = 0;

        private bool isEditingHours = false;

        private DateTime Start = DateTime.Today;
        private DateTime End = DateTime.Today;
        private int SelectedEmployeeID { get; set; }

        private bool IsAddShiftToEmployeeWithoutAvaDialogVisible { get; set; } = false;
        private bool Editable { get; set; } = false;


        #region ToastAndNotification
        private SfToast Toast = new();
        private bool IsDeleteDialogVisible { get; set; } = false;
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Kierownik"))
                NavManager.NavigateTo("/auth-error");

            await GetSchedule();
            CurrentDate = DailySchedule.Date;
            await GetEmployees();
            await GetAvailabilities();
            await GetUserSchedules();
            await GetWork();
            await LoadDataToSchedule();
            await InvokeAsync(StateHasChanged);

              if (DailySchedule.Status == 1 || DailySchedule.Date < DateTime.Now)
              {
                  Editable = false;

                if(DailySchedule.Date < DateTime.Now)
                {
                    DailySchedule.Status = 1;
                }
                
              }
              else
              {
                  Editable = true;
              } 
            

            await base.OnInitializedAsync();
        }
        
        //Schedule

        #region calendar
        private async Task LoadDataToSchedule()
        {
            var updatedResources = new List<EmployeeResource>();

            foreach (UsersResponse user in Employees)
            {
                var employeeResource = new EmployeeResource
                {
                    Name = $"{user.FirstName} {user.LastName}",
                    Id = user.ID,
                    Color = ""
                };
                updatedResources.Add(employeeResource);
            }

            EmployeeResources = updatedResources;
            await InvokeAsync(StateHasChanged);

            int i = 10000;

            var updatedShiftAppointments = new List<ShiftAppointment>();

            foreach (UsersDailySchedulesResponse userShift in UsersDailySchedulesResponses)
            {

                var shiftAppointment = new ShiftAppointment { Id = userShift.ID, Subject = "Zmiana", StartTime = userShift.TimeStart, EndTime = userShift.TimeEnd, EmployeeID = userShift.UserID, CssClass = "shift" };

                updatedShiftAppointments.Add(shiftAppointment);

            }
         

            foreach (AvailabilitiesResponse userAva in Availabilities)
            {

                var startTime = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day,
                             userAva.TimeStart.Hour, userAva.TimeStart.Minute, userAva.TimeStart.Second);

                var endTime = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day,
                                           userAva.TimeEnd.Hour, userAva.TimeEnd.Minute, userAva.TimeEnd.Second);


                var shiftAppointment = new ShiftAppointment { Id = i, Subject = "Dyspozycyjność", StartTime = startTime, EndTime = endTime, EmployeeID = userAva.UserID, CssClass = "ava" };

                updatedShiftAppointments.Add(shiftAppointment);
                i++;
            }
            if (DailySchedule.Date < DateTime.Now)
            {
                
                foreach (WorksResponse userWork in Works)
                {

                    var startTime = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day,
                                 userWork.TimeStart.Hour, userWork.TimeStart.Minute, userWork.TimeStart.Second);

                    var endTime = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day,
                                               userWork.TimeEnd.Hour, userWork.TimeEnd.Minute, userWork.TimeEnd.Second);

                    
                    var shiftAppointment = new ShiftAppointment { Id = i, Subject = "Czas pracy", StartTime = startTime, EndTime = endTime, EmployeeID = userWork.UserID, CssClass = "work" };

                    updatedShiftAppointments.Add(shiftAppointment);
                    i++;
                }

            }
            ShiftAppointments = updatedShiftAppointments;
            await InvokeAsync(StateHasChanged);

        }
        #endregion


        #region GetFromDataBase

        private async Task GetSchedule()
        {

            var responseSchedule = await ScheduleService.GetDailySchedule(ID);
            if (responseSchedule.Success)
            {
                DailySchedule = responseSchedule.Data!;
            }
            else
            {
               // await ShowToast(responseSchedule.Message!, responseSchedule.Success);
                return;
            }

            var responseHours = await ScheduleService.GetHoursForDayForTeam(DailySchedule.TeamID, DailySchedule.Date);

            if (responseHours.Success)
            {
                TotalHours = responseHours.Data;
            }
            else
            {
               // await ShowToast(responseHours.Message!, responseHours.Success);
                return;
            }

        }
        private async Task GetEmployees()
        {
            var response = await UsersService.GetUserByTeam(DailySchedule.TeamID);
            if (response.Success)
            {
                Employees = response.Data!;
            }
            else
            {
                await Task.Delay(100);
                await ShowToast(response.Message!, response.Success);
            }
        }

        private async Task GetUserSchedules()
        {
            var response = await ScheduleService.GetUsersDailySchedules(DailySchedule.TeamID, DailySchedule.Date);
            if (response.Success)
            {
                UsersDailySchedulesResponses = response.Data!;
            }
            else
            {
               // await ShowToast(response.Message!, response.Success);
            }
        }

        private async Task GetAvailabilities()
        {
            var response = await AvailabilitiesService.GetAvailabilitiesForTeam(DailySchedule.Date, DailySchedule.TeamID);
            if (response.Success)
            {
                Availabilities = response.Data!;
            }
            else
            {
               // await ShowToast(response.Message!, response.Success);
            }
        }

        private async Task GetWork()
        {

            if (DailySchedule.Date < DateTime.Now)
            {
                var response = await WorksService.GetWorksForTeamForDay(DailySchedule.Date, DailySchedule.TeamID);
                if (response.Success)
                {
                    Works = response.Data!;
                }
                else
                {
                  //  await ShowToast(response.Message!, response.Success);

                }
            }

        }

        #endregion

        #region CalendarEditShift


        public async Task OnOpen(BeforeOpenCloseMenuEventArgs<MenuItem> args)
        {
            if (args.ParentItem != null)
            {
                return; 
            }

            ElementData = await ScheduleRef.GetElementInfoAsync((int)args.Left!, (int)args.Top!);

            if (ElementData.ElementType != ElementType.Event)
            {
                args.Cancel = true; 
            }
            else
            {
                EventData = ElementData.EventData;
                if (EventData == null || EventData.Id == 0)
                {
                    args.Cancel = true; 
                }
            }
        }


        public async Task OnItemSelected(MenuEventArgs<MenuItem> args)
        {
            if (DailySchedule.Status == 1)
            {
                await Task.Delay(100);
                await ShowToast("Nie możesz edytować zmian w zamkniętym dniu.", false);
                return;
            }

            if (DailySchedule.Date < DateTime.Now)
            {
                await Task.Delay(100);
                await ShowToast("Nie możesz edytować zmian z przeszłości.", false);
                return;
            }

            if (args.Item.Id == "Details" && EventData != null)
            {
                if (EventData.Subject == "Zmiana")
                {
                    var userShift = UsersDailySchedulesResponses.Find(x => x.ID == EventData.Id);
                    if (userShift != null)
                    {
                        Start = userShift.TimeStart;
                        End = userShift.TimeEnd;
                        await OpenShiftDialog(userShift);

                    }
                }
                else if (EventData.Subject == "Dyspozycyjność")
                {
                    var userAva = Availabilities.Find(x => x.UserID == EventData.EmployeeID);
                    if (userAva != null)
                    {
                        var userShift = UsersDailySchedulesResponses.Find(x => x.UserID == userAva.UserID);

                        if (userShift == null)
                        {
                            ShiftToEdit = new UsersDailySchedulesResponse();
                            ShiftToEdit.UserID = userAva.UserID;
                            ShiftToEdit.TimeStart = userAva.TimeStart;
                            ShiftToEdit.TimeEnd = userAva.TimeEnd;
                            ShiftToEdit.IsEditing = false;
                            await OpenShiftDialog(ShiftToEdit);


                        }



                    }
                }

            }
        }

        private async Task OpenShiftDialog(UsersDailySchedulesResponse userShift)
        {
            IsShiftDialogVisible = true;
            ShiftToEdit = userShift;
            await InvokeAsync(StateHasChanged);
        }

        private async Task SaveEditedShiftAsync()
        {
            var updatedTimeStart = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                                    Start.Hour, Start.Minute, 0);
            var updatedTimeEnd = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                                              End.Hour, End.Minute, 0);


            if (updatedTimeStart > updatedTimeEnd || (updatedTimeEnd - updatedTimeStart).TotalHours < 1)
            {
                await Task.Delay(100);
                await ShowToast("Nie poprawne godziny.", false);
                return;
            }



            
            var response = await ScheduleService.UpdateUserSchedule(new UsersDailySchedulesRequest
            {

                ID = ShiftToEdit.ID,
                TimeStart = updatedTimeStart,
                TimeEnd = updatedTimeEnd,
                UserID = ShiftToEdit.UserID
            });
            if (response.Success)
            {
                ShiftToEdit.IsEditing = false;
                await RefreshHoursAsync();
                await Task.Delay(100);
                await ShowToast("Zmiany zostały zapisane.", true);
                IsShiftDialogVisible = false;
                await InvokeAsync(StateHasChanged);
                NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
            }
            else
            {
                await Task.Delay(100);
                await ShowToast(response.Message!, false);
            }


        }
        private async Task AddShiftAsync()
        {
            var employee = Employees.Find(x => x.ID == ShiftToEdit.UserID);
            var updatedTimeStart = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                            Start.Hour, Start.Minute, 0);
            var updatedTimeEnd = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                                              End.Hour, End.Minute, 0);

            if (updatedTimeStart > updatedTimeEnd || (updatedTimeEnd - updatedTimeStart).TotalHours < 1)
            {
                await Task.Delay(100);
                await ShowToast("Nie poprawne godziny.", false);
                return;
            }
            
            var newShift = new UsersDailySchedulesRequest
            {
                UserID = employee!.ID,
                TimeStart = updatedTimeStart,
                TimeEnd = updatedTimeEnd
            };

            var response = await ScheduleService.AddUserSchedule(newShift);
            if (response.Success)
            {
                employee.IsAddingShift = false;
                await GetUserSchedules();
                await RefreshHoursAsync();
                await Task.Delay(100);
                await ShowToast("Zmiana została dodana.", true);
                IsShiftDialogVisible = false;
                await InvokeAsync(StateHasChanged);
                NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
            }
            else
            {
                await Task.Delay(100);
                await ShowToast(response.Message!, false);
            }


        }

        private void CancelEditShift()
        {
            IsShiftDialogVisible = false;


        }

        private async Task DeleteShiftAsync()
        {
            
            var response = await ScheduleService.DeleteUserSchedule(ShiftToEdit.ID);
            if (response.Success)
            {
                await RefreshHoursAsync();
                await Task.Delay(100);
                await ShowToast("Zmiana została usunięta.", true);
                IsShiftDialogVisible = false;
                await InvokeAsync(StateHasChanged);
                NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
            }
            else
            {
                await Task.Delay(100);
                await ShowToast(response.Message!, false);
            }
        }


        #endregion

        #region EditHours, DeleteShift, AddShift

        private void EnableEditHours()
        {
            isEditingHours = true;
        }

        private async Task SaveHoursAsync()
        {
            var response = await ScheduleService.UpdateSchedule(new DailySchedulesRequest
            {
                ID = DailySchedule.ID,
                Date = DailySchedule.Date,
                HoursAmount = DailySchedule.HoursAmount,
                Status = DailySchedule.Status,
                TeamID = DailySchedule.TeamID
            });
            if (response.Success)
            {
                await Task.Delay(100);
                await ShowToast("Godziny zostały zaktualizowane.", true);
                isEditingHours = false;
            }
            else
            {
                await Task.Delay(100);
                await ShowToast(response.Message!, false);
            }
        }

        private void CancelEditHours()
        {
            isEditingHours = false;
        }


        private async Task RefreshHoursAsync()
        {
            var responseHours = await ScheduleService.GetHoursForDayForTeam(DailySchedule.TeamID, DailySchedule.Date);
            if (responseHours.Success)
            {
                TotalHours = responseHours.Data;
                await Task.Delay(100);
                await ShowToast("Ilość godzin została odświeżona.", true);
            }
            else
            {
                await Task.Delay(100);
                await ShowToast(responseHours.Message!, false);
            }
        }

        private async Task ConfirmDeleteSchedule()
        {
            IsDeleteDialogVisible = false;
            var response = await ScheduleService.DeleteSchedule(DailySchedule.ID);

            if (response.Success)
            {
                DailySchedule = new DailySchedulesResponse();
                await Task.Delay(100);
                await ShowToast(response.Message!, response.Success);
                NavManager.NavigateTo("/ScheduleMonth");
            }
            else
            {
                await Task.Delay(100);
                await ShowToast(response.Message!, response.Success);
            }
        }



        private async Task AddShiftIfNoAvailabilitieAsync()
        {
            var employee = Employees.Find(x => x.ID == SelectedEmployeeID);
            var updatedTimeStart = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                            Start.Hour, Start.Minute, 0);
            var updatedTimeEnd = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                                              End.Hour, End.Minute, 0);

            if (updatedTimeStart > updatedTimeEnd || (updatedTimeEnd - updatedTimeStart).TotalHours < 1)
            {
                await Task.Delay(100);
                await ShowToast("Nie poprawne godziny.", false);
                return;
            }
            
            var newShift = new UsersDailySchedulesRequest
            {
                UserID = employee!.ID,
                TimeStart = updatedTimeStart,
                TimeEnd = updatedTimeEnd
            };

            var response = await ScheduleService.AddUserSchedule(newShift);
            if (response.Success)
            {
                employee.IsAddingShift = false;
                await GetUserSchedules();
                await RefreshHoursAsync();
                await Task.Delay(100);
                await ShowToast("Zmiana została dodana.", true);
                IsAddShiftToEmployeeWithoutAvaDialogVisible = false;
                await InvokeAsync(StateHasChanged);
                NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
            }
            else
            {
                await Task.Delay(100);
                await ShowToast(response.Message!, false);
            }


        }

        private void CancelAddShift()
        {
            IsAddShiftToEmployeeWithoutAvaDialogVisible = false;
        }

        private async Task OpenAddShift()
        {
            IsAddShiftToEmployeeWithoutAvaDialogVisible = true;
            await InvokeAsync(StateHasChanged);
        }





        #endregion

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
        private void ShowDeleteConfirmation()
        {
            IsDeleteDialogVisible = true;
        }

        private void CloseDeleteDialog()
        {
            IsDeleteDialogVisible = false;
        }
        #endregion
    }
}
