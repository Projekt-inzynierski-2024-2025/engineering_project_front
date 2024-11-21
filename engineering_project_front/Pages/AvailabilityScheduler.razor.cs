using engineering_project_front.Layout;
using engineering_project_front.Models;
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

        [Inject]
        private IAvailabilitiesService availabilitiesService { get; set; } = default!;

        protected async override Task OnInitializedAsync()
        {
            await GetAvailabilities();

            CreateTree();

            await base.OnInitializedAsync();
        }

        private async Task GetAvailabilities()
        {
            var responseCurrentMonth = await availabilitiesService.GetAvailabilitiesForMonth(CurrentDate);
            var responseNextMonth = await availabilitiesService.GetAvailabilitiesForMonth(CurrentDate.AddMonths(1));

            if (responseCurrentMonth.Success && responseNextMonth.Success)
            {
                dataSource = responseCurrentMonth.Data!.ToList();
                dataSource.AddRange(responseNextMonth.Data!.ToList());
            }
        }

        public async Task OnCellClick(CellClickEventArgs args)
        {
            args.Cancel = true;
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
                new TreeData()
                {
                    Id= "6",
                    Pid = "1",
                    Name = "Zmień godziny pracy"
                },
                new TreeData()
                {
                    Id = "7",
                    Pid = "1",
                    Name = "Sprawdź dostępności godzinowe",
                    Selected = true,
                }
            };

        }
    }
}
