using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using engineering_project_front.Models.Request;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class AddSchedule: ComponentBase
    {
        #region Injects
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        [Inject]
        private IScheduleService ScheduleService { get; set; } = default!;
        [Inject]
        private IValidateRole validateRole { get; set; } = default!;
        #endregion
        [Parameter]
        public required string ParamID { get; set; }
        private long ID => long.Parse(ParamID);

        private DailySchedulesRequest Schedule { get; set; } = new DailySchedulesRequest();

        DateTime minDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);

        #region Toast
        private SfToast Toast = default!;
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Kierownik"))
                NavManager.NavigateTo("/auth-error");

            Schedule.Date = DateTime.Now.AddDays(1);

            Schedule.TeamID = ID;
            await InvokeAsync(StateHasChanged);
        }

        private async Task HandleValidSubmit()
        {
           
                var response = await ScheduleService.AddSchedule(Schedule);
                if (response.Success)
                {
                    await ShowToast(response.Message!, response.Success);
                    await Task.Delay(2000);
                    NavManager.NavigateTo("/ScheduleMonth");
                }
                else
                {
                    await ShowToast(response.Message!, response.Success);
                }
            

        }

        private void Cancel()
        {
            NavManager.NavigateTo("/ScheduleMonth");
        }

        #region Toast
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
    }
}
