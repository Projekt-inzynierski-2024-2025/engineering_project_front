using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using engineering_project_front.Models.Request;
using Syncfusion.Blazor.Notifications;
using engineering_project_front.Models.Responses;
using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Services;

namespace engineering_project_front.Pages
{
    public partial class AddSchedule: ComponentBase
    {
        #region Injects
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        [Inject]
        private IScheduleService ScheduleService { get; set; } = default!;
        #endregion
        [Parameter]
        public required string ParamID { get; set; }
        private long ID => long.Parse(ParamID);

        private DailySchedulesRequest Schedule { get; set; } = new DailySchedulesRequest();

        #region Toast
        private SfToast? Toast;
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Schedule.TeamID = ID;
        }

        private async Task HandleValidSubmit()
        {
           
                var response = await ScheduleService.AddSchedule(Schedule);
                if (response.Success)
                {
                    ShowToast(response.Message, response.Success);
                    await Task.Delay(2000);
                    NavManager.NavigateTo("/ScheduleMonth");
                }
                else
                {
                    ShowToast(response.Message, response.Success);
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
            await Toast?.ShowAsync();
        }

        #endregion
    }
}
