using engineering_project_front.Layout;
using engineering_project_front.Models;
using Syncfusion.Blazor.Calendars;

namespace engineering_project_front.Pages
{
    public partial class EditWork
    {
        protected async override Task OnInitializedAsync()
        {
            CreateTree();

            await base.OnInitializedAsync();
        }
        private SfDatePicker<DateTime?> DatePickerObj { get; set; } = default!;
        private void DisableDate(RenderDayCellEventArgs args)
        {
            var view = DatePickerObj.CurrentView();
            if (view == "Month" && ((int)args.Date.DayOfWeek == 0 || (int)args.Date.DayOfWeek == 6))
            {
                args.IsDisabled = true;
            }

            if(args.Date > DateTime.Today)
            {
                args.IsDisabled = true;
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
                }
            };

        }
    }
}
