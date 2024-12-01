using Blazored.SessionStorage;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
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
        private ISessionStorageService SessionStorage { get; set; } = default!;
        #endregion


        [Parameter]
        public long? UserID { get; set; }
        private bool IsManager { get; set; } = false;
        private List<UsersDailySchedulesResponse> UserShifts { get; set; } = new();

        #region ToastAndNotification
        private SfToast? Toast;
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        #endregion



        protected override async Task OnParametersSetAsync()
        {
            if (UserID.HasValue)
            {
                // Kierownik przegląda zmiany pracownika
                IsManager = true;
                var response = await ScheduleService.GetUsersDailySchedulesForMonth(UserID.Value, DateTime.Now);

                if (response.Success)
                {
                    UserShifts= response.Data;
                }
                else
                {
                    ShowToast(response.Message, response.Success);
                    
                }
                 
            }
            else
            {
                // Pracownik przegląda swoje zmiany
                IsManager = false;
                UserShifts = await GetUsersDailySchedulesForMonth();
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





        private async Task<List<UsersDailySchedulesResponse>> GetUsersDailySchedulesForMonth()
        {
            var token = await SessionStorage.GetItemAsStringAsync("token");

            if (string.IsNullOrEmpty(token))
                return null;

            token = token.Trim('"');

            var user = await UsersService.GetUserFromToken(token);
            var response = await ScheduleService.GetUsersDailySchedulesForMonth(user.ID,DateTime.Now);

            if (response.Success)
            {
                return response.Data;
            }
            else
            {
                ShowToast(response.Message, response.Success);
                return null;
            }
        }



        private void OnContextMenuClick(ContextMenuClickEventArgs<UsersDailySchedulesResponse> args)
        {
            ShowToast("response.Message", true);
        }
    }
}
