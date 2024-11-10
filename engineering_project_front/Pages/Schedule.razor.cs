using engineering_project_front.Layout;
using engineering_project_front.Models;
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

        #region ToastAndNotification
        private SfToast? Toast;
        private bool IsDeleteDialogVisible { get; set; } = false;
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            CreateTree();
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
            var response = await ScheduleService.UpdateHoursAmount(DailySchedule.TeamID, DailySchedule.Date, DailySchedule.HoursAmount);
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
            var response = await ScheduleService.UpdateStatus(DailySchedule.TeamID, DailySchedule.Date, DailySchedule.Status);
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
        private void CreateTree()
        {
            SidebarMenu.Instance.TreeData = new()
            {
                new TreeData
                {
                    Id = "1",
                    Name = "Ogólne",
                    HasChild = true,
                    Expanded = true,
                },
                new TreeData
                {
                    Id = "2",
                    Pid = "1",
                    Name = "Strona głowna",
                },
                new TreeData
                {
                    Id = "3",
                    Pid = "1",
                    Name = "Login"
                },
                new TreeData
                {
                    Id = "4",
                    Pid = "1",
                    Name = "Zarządzanie użytkownikami",
                },
                new TreeData
                {
                    Id = "5",
                    Pid = "1",
                    Name = "Zarządzanie zespołami",
                },
                new TreeData
                {
                    Id = "6",
                    Pid = "1",
                    Name = "Grafik",
                }
            };
        }

    }
}
