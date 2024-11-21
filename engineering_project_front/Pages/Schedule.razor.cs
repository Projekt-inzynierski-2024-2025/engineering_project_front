using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Models.Request;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace engineering_project_front.Pages
{
    public partial class Schedule:ComponentBase
    {
        #region Injects
        [Inject]
        private IScheduleService ScheduleService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        [Inject]
        private IUsersService UsersService { get; set; } = default!;
        #endregion

        [Parameter]
        public required string ParamID { get; set; }

        private long ID => long.Parse(ParamID);

        private DailySchedulesResponse DailySchedule { get; set; } = new DailySchedulesResponse();

        private Double TotalHours { get; set; } = 0;

        private List<UsersResponse> Employees { get; set; } = new List<UsersResponse>();
        private List<UsersDailySchedulesResponse> EmployeesShift { get; set; } = new List<UsersDailySchedulesResponse>();

        private bool isEditingHours = false;

        private DateTime Start = DateTime.Today;
        private DateTime End = DateTime.Today;

        #region ToastAndNotification
        private SfToast? Toast;
        private bool IsDeleteDialogVisible { get; set; } = false;
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            await GetSchedule();
            await GetEmployees();
            await GetUserSchedules();
            await base.OnInitializedAsync();
        }

        private async Task GetSchedule()
        {

            var responseSchedule = await ScheduleService.GetDailySchedule(ID);
            if (responseSchedule.Success) {
                DailySchedule = responseSchedule.Data;
            }
            else
            {
                ShowToast(responseSchedule.Message, responseSchedule.Success);
                return;
            }

            var responseHours = await ScheduleService.GetHoursForDayForTeam(DailySchedule.TeamID, DailySchedule.Date);

            if (responseHours.Success)
            {
                TotalHours = responseHours.Data;
            }
            else
            {
                ShowToast(responseHours.Message, responseHours.Success);
                return;
            }

        }

        private async Task GetEmployees()
        {
            var response = await UsersService.GetUserByTeam(DailySchedule.TeamID);
            if (response.Success)
            {
                Employees = response.Data;
            }
            else
            {
                ShowToast(response.Message, response.Success);
            }
        }

        private async Task GetUserSchedules()
        {
            var response = await ScheduleService.GetUsersDailySchedules(DailySchedule.TeamID, DailySchedule.Date);
            if (response.Success)
            {
                EmployeesShift = response.Data;
            }
            else
            {
                ShowToast(response.Message, response.Success);
            }
        }

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
                ShowToast("Godziny zostały zaktualizowane.", true);
                isEditingHours = false;
            }
            else
            {
                ShowToast(response.Message, false);
            }
        }

        private void CancelEditHours()
        {
            isEditingHours = false;
        }

        private async Task ToggleStatus()
        {
            DailySchedule.Status = DailySchedule.Status == 0 ? 1 : 0;
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
                ShowToast("Status został zaktualizowany.", true);
            }
            else
            {
                ShowToast(response.Message, false);
            }
        }

        private async Task RefreshHoursAsync()
        {
            var responseHours = await ScheduleService.GetHoursForDayForTeam(DailySchedule.TeamID, DailySchedule.Date);
            if (responseHours.Success)
            {
                TotalHours = responseHours.Data;
                ShowToast("Ilość godzin została odświeżona.", true);
            }
            else
            {
                ShowToast(responseHours.Message, false);
            }
        }





        private void EnableEditShift(UsersDailySchedulesResponse employeeShift)
        {
            employeeShift.IsEditing = true;
        }

        private async Task SaveShiftAsync(UsersDailySchedulesResponse employeeShift)
        {
            var updatedTimeStart = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                                    employeeShift.TimeStart.Hour, employeeShift.TimeStart.Minute, 0);
            var updatedTimeEnd = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                                              employeeShift.TimeEnd.Hour, employeeShift.TimeEnd.Minute, 0);


            // Save the edited shift data to the backend
            var response = await ScheduleService.UpdateUserSchedule (new UsersDailySchedulesRequest
            {
                
                ID = employeeShift.ID,
                TimeStart = updatedTimeStart,
                TimeEnd = updatedTimeEnd,
                UserID = employeeShift.UserID
            });
            if (response.Success)
            {
                employeeShift.IsEditing = false;
                await RefreshHoursAsync();
                ShowToast("Zmiany zostały zapisane.", true);
            }
            else
            {
                ShowToast(response.Message, false);
            }
        }

        private void CancelEditShift(UsersDailySchedulesResponse employeeShift)
        {
            employeeShift.IsEditing = false;
        }

        private void EnableAddShift(UsersResponse employee)
        {
            employee.IsAddingShift = true;
        }

        private async Task AddShiftAsync(UsersResponse employee)
        {
            var updatedTimeStart = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                            Start.Hour, Start.Minute, 0);
            var updatedTimeEnd = new DateTime(DailySchedule.Date.Year, DailySchedule.Date.Month, DailySchedule.Date.Day,
                                              End.Hour, End.Minute, 0);

            // Save the new shift data to the backend
            var newShift = new UsersDailySchedulesRequest
            {
                UserID = employee.ID,
                TimeStart = updatedTimeStart,
                TimeEnd = updatedTimeEnd
            };

            var response = await ScheduleService.AddUserSchedule(newShift);
            if (response.Success)
            {
                employee.IsAddingShift = false;
                await GetUserSchedules();
                await RefreshHoursAsync();
                ShowToast("Zmiana została dodana.", true);
            }
            else
            {
                ShowToast(response.Message, false);
            }
        }

        private void CancelAddShift(UsersResponse employee)
        {
            employee.IsAddingShift = false;
        }

        private async Task DeleteShiftAsync(UsersDailySchedulesResponse employeeShift)
        {
            // Delete the shift data from the backend
            var response = await ScheduleService.DeleteUserSchedule(employeeShift.ID);
            if (response.Success)
            {
                EmployeesShift.Remove(employeeShift);
                await RefreshHoursAsync();
                ShowToast("Zmiana została usunięta.", true);
            }
            else
            {
                ShowToast(response.Message, false);
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
            await Toast?.ShowAsync();
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
