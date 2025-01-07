using Blazored.SessionStorage;
using engineering_project_front.Models.Request;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class EditWork
    {
        #region Injections
        [Inject]
        private IValidateRole validateRole { get; set; } = default!;

        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        [Inject]
        private ISessionStorageService sessionStorage { get; set; } = default!;

        [Inject]
        private IUsersService usersService { get; set; } = default!;

        [Inject]
        private IWorksService worksService { get; set; } = default!;
        #endregion

        private long ID = -1;

        SfToast ToastObj = default!;
        private string ToastContent = string.Empty;

        private SfDatePicker<DateTime?> datePickerObj { get; set; } = default!;
        private DateTime? datePicker { get; set; } = DateTime.Today;
        private DateTime minDate = new(DateTime.Today.Year, DateTime.Today.Month, 1);
        private DateTime maxDate = new(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));


        private DateTime? workStart { get; set; }
        private DateTime? breakStart { get; set; }
        private DateTime? workEnd { get; set; }
        private DateTime? breakEnd { get; set; }

        private bool disablePickingTime = true;
        protected async override Task OnInitializedAsync()
        {
            if (!await validateRole.IsAuthorized("Kierownik", "Pracownik"))
                navManager.NavigateTo("/auth-error");

            await GetUser();

            await GetWorkToEdit();

            await base.OnInitializedAsync();
        }

        private async Task GetWorkToEdit()
        {
            if (ID == -1) return;

            if (datePicker == null) return;

            var result = await worksService.GetWorkForDay(ID, datePicker.Value);

            if (result.Success)
            {
                var data = result.Data!;

                if (datePicker != DateTime.Today && data.Status != 0)
                {
                    workStart = null;
                    workEnd = null;
                    breakStart = null;
                    breakEnd = null;
                    ToastContent = "Podanej pracy nie można edytować. Skontaktuj się z kierownikiem, by umożliwił ci edycję.";
                    disablePickingTime = true;
                    await InvokeAsync(StateHasChanged);
                    await ToastObj.ShowAsync();
                    return;
                }

                workStart = data.TimeStart;

                if (data.TimeEnd != DateTime.MinValue)
                {
                    disablePickingTime = false;
                    workEnd = data.TimeEnd;
                }
                else
                    workEnd = new DateTime(data.Date.Year, data.Date.Month, data.Date.Day, 0, 0, 0);

                if (data.BreakStart != DateTime.MinValue)
                    breakStart = data.BreakStart;
                else
                    breakStart = new DateTime(data.Date.Year, data.Date.Month, data.Date.Day, 0, 0, 0);

                if (data.BreakEnd != DateTime.MinValue)
                    breakEnd = data.BreakEnd;
                else
                    breakEnd = new DateTime(data.Date.Year, data.Date.Month, data.Date.Day, 0, 0, 0);
            }
            else
            {
                workStart = null;
                workEnd = null;
                breakStart = null;
                breakEnd = null;
                ToastContent = "Brak pracy do edycji.";
                disablePickingTime = true;
                await InvokeAsync(StateHasChanged);
                await ToastObj.ShowAsync();
            }

            await InvokeAsync(StateHasChanged);
        }

        private bool ValidateTimes()
        {
            if (workStart > workEnd)
            {
                ToastContent = "Czas rozpoczęcia pracy jest późniejszy niż czas zakończenia pracy. Proszę to zmienić.";
                InvokeAsync(StateHasChanged);
                ToastObj.ShowAsync();
                return false;
            }

            if (!(breakStart!.Value.TimeOfDay == new TimeSpan(0, 0, 0) && breakEnd!.Value.TimeOfDay == new TimeSpan(0, 0, 0)))
            {
                if (breakEnd < breakStart)
                {
                    ToastContent = "Czas rozpoczęcia przerwy jest późniejszy niż czas zakończenia przerwy. Proszę to zmienić.";
                    InvokeAsync(StateHasChanged);
                    ToastObj.ShowAsync();
                    return false;
                }

                if (breakStart < workStart)
                {
                    ToastContent = "Czas rozpoczęcia przerwy jest późniejszy niż czas rozpoczęcia pracy. Proszę to zmienić.";
                    InvokeAsync(StateHasChanged);
                    ToastObj.ShowAsync();
                    return false;
                }

                if (breakStart > workEnd)
                {
                    ToastContent = "Czas rozpoczęcia przerwy jest późniejszy niż czas zakończenia pracy. Proszę to zmienić.";
                    InvokeAsync(StateHasChanged);
                    ToastObj.ShowAsync();
                    return false;
                }

                if (breakEnd > workEnd)
                {
                    ToastContent = "Czas zakończenia przerwy jest późniejszy niż czas zakończenia pracy. Proszę to zmienić.";
                    InvokeAsync(StateHasChanged);
                    ToastObj.ShowAsync();
                    return false;
                }

                if (breakEnd < workStart)
                {
                    ToastContent = "Czas rozpoczęcia pracy jest późniejszy niż czas zakończenia przerwy. Proszę to zmienić.";
                    InvokeAsync(StateHasChanged);
                    ToastObj.ShowAsync();
                    return false;
                }
            }

            return true;
        }

        private async Task EditWorkTimes()
        {
            if (datePicker == null || ID == -1) return;
            await InvokeAsync(StateHasChanged);

            if (!ValidateTimes()) return;

            WorksRequest request = new()
            {
                UserID = ID,
                Date = datePicker.Value,

                TimeStart = workStart!.Value,
                TimeEnd = workEnd!.Value,
                BreakStart = breakStart!.Value,
                BreakEnd = breakEnd!.Value,
                Status = 1,
            };

            var result = await worksService.EditWork(request);
            if (result.Success)
            {
                ToastContent = result.Message!;
                if (DateTime.Today != request.Date.Date)
                    disablePickingTime = true;
                await InvokeAsync(StateHasChanged);
                await ToastObj.ShowAsync();
            }
        }

        private async Task OnDateChanged()
        {
            await GetWorkToEdit();

        }

        private async Task GetUser()
        {
            var token = await sessionStorage.GetItemAsStringAsync("token");

            if (token == null)
                return;

            token = token.Trim('"');

            var user = await usersService.GetUserFromToken(token);

            ID = user.ID!;
        }

        private void DisableDate(RenderDayCellEventArgs args)
        {
            var view = datePickerObj.CurrentView();
            if (view == "Month" && ((int)args.Date.DayOfWeek == 0 || (int)args.Date.DayOfWeek == 6))
            {
                args.IsDisabled = true;
            }

            if (args.Date > DateTime.Today)
            {
                args.IsDisabled = true;
            }
        }
    }
}
