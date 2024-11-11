using Blazored.SessionStorage;
using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class MyEmployees:ComponentBase
    {
        #region Injection
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private IScheduleService ScheduleService { get; set; } = default!;
        [Inject]
        private IUsersService UsersService { get; set; } = default!;
        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        [Inject]
        private ISessionStorageService SessionStorage { get; set; } = default!;
        #endregion

        private List<HoursForUserForMonthResponse> HoursForUsers { get; set; } = new();
        private long TeamID { get; set; } = 0;

        #region ToastAndNotification
        private SfToast? Toast;
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            CreateTree();

            TeamID = await GetTeamID();

            var response = await ScheduleService.GetUsersHoursForMonth(DateTime.Now.Year, DateTime.Now.Month, TeamID);
            if (response.Success)
            {
                HoursForUsers = response.Data;
            }
            else
            {
                ShowToast(response.Message, response.Success);
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

        #endregion


        private async Task<long> GetTeamID()
        {
            var token = await SessionStorage.GetItemAsStringAsync("token");

            if (string.IsNullOrEmpty(token))
                return 0;

            token = token.Trim('"');

            var user = await UsersService.GetUserFromToken(token);
            var response = await TeamsService.GetTeamIDForManager(user.Email);

            if (response.Success)
            {
                return response.Data;
            }
            else
            {
                ShowToast(response.Message, response.Success);
                return 0;
            }
        }
        private void CreateTree()
        {
            SidebarMenu.Instance.TreeData =
            [
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
                    Name = "Login",
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
                },
                new TreeData
                {
                    Id = "7",
                    Pid = "1",
                    Name = "Moi Pracownicy",
                    Selected = true
                }
            ];
        }




    }
}
