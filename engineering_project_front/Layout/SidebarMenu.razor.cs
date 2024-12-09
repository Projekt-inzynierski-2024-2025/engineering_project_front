using Blazored.SessionStorage;
using engineering_project_front.Models;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;

namespace engineering_project_front.Layout
{
    public partial class SidebarMenu
    {
        #region Injection
        [Inject]
        NavigationManager NavManager { get; set; } = default!;
        [Inject]
        ISessionStorageService sessionStorage { get; set; } = default!;
        #endregion

        public static SidebarMenu Instance { get; private set; } = default!;

        private bool SidebarToggle = false;

        private string Role = string.Empty;

        public List<TreeData> TreeData { get; set; } = new();


        protected async override Task OnInitializedAsync()
        {
            Instance = this;

            Role = await sessionStorage.GetItemAsStringAsync("role");

            CreateTree();

            await base.OnInitializedAsync();
        }

        private void OnMenuClick()
        {
            SidebarToggle = !SidebarToggle;
        }

        public static void BeforeSelect(NodeSelectEventArgs args)
        {
            switch (args.NodeData.Id)
            {
                case "1":
                    args.Cancel = true;
                    break;
                default:
                    break;
            }
        }

        public virtual void CreateTree()
        {
            //Overall
            TreeData = new()
            {
                new TreeData
                {
                    Id = "1",
                    Name = "Ogólne",
                    HasChild = true,
                    Expanded = true,
                },
                new TreeData()
                {
                    Id = "4",
                    Pid = "1",
                    Name = "Moje konto"
                },
                new TreeData()
                {
                    Id = "5",
                    Pid = "1",
                    Name = "Wyloguj się"
                },
            };

            if (Role == "Administrator")
            {
                //For Admin
                TreeData.AddRange(new List<TreeData>{
                    new TreeData
                    {
                        Id = "3",
                        Name = "Panel Administracyjny",
                        HasChild = true,
                        Expanded = true,
                    },
                    new TreeData
                    {
                        Id = "13",
                        Pid = "3",
                        Name = "Zarządzanie użytkownikami",
                    },
                    new TreeData
                    {
                        Id = "14",
                        Pid = "3",
                        Name = "Zarządzanie zespołami",
                    },
                });
            }
            else
            {
                //For Employee
                TreeData.AddRange(new List<TreeData>
                {
                    new TreeData
                    {
                        Id = "6",
                        Pid = "1",
                        Name = "Strona głowna",
                    },
                    new TreeData()
                    {
                        Id= "7",
                        Pid = "1",
                        Name = "Zmień godziny pracy"
                    },
                    new TreeData()
                    {
                        Id = "8",
                        Pid = "1",
                        Name = "Dostępności godzinowe"
                    },
                    new TreeData()
                    {
                        Id = "9",
                        Pid = "1",
                        Name = "Czas pracy"
                    },
                    new TreeData()
                    {
                        Id = "10",
                        Pid = "1",
                        Name = "Moje zmiany"
                    },
                });

                //For Manager
                if (Role == "Kierownik")
                {
                    TreeData.AddRange(new List<TreeData>{
                    new TreeData
                    {
                        Id = "2",
                        Name = "Panel Kierownika",
                        HasChild = true,
                        Expanded = true,
                    },
                    new TreeData
                    {
                        Id = "11",
                        Pid = "2",
                        Name = "Moi Pracownicy",
                    },
                    new TreeData
                    {
                        Id = "12",
                        Pid = "2",
                        Name = "Grafik",
                    },
                });

                }
            }
        }

        public void OnSelect(NodeSelectEventArgs args)
        {
            switch (args.NodeData.Id)
            {
                case "4":
                    NavManager.NavigateTo("/my-account");
                    break;
                case "5":
                    sessionStorage.RemoveItemAsync("token");
                    sessionStorage.RemoveItemAsync("role");
                    NavManager.NavigateTo("/");
                    break;
                case "6":
                    NavManager.NavigateTo("/home");
                    break;
                case "7":
                    NavManager.NavigateTo("/edit-work");
                    break;
                case "8":
                    NavManager.NavigateTo("/availability-scheduler");
                    break;
                case "9":
                    NavManager.NavigateTo("/show-time");
                    break;
                case "10":
                    NavManager.NavigateTo("/myShifts");
                    break;
                case "11":
                    NavManager.NavigateTo("/MyEmployees");
                    break;
                case "12":
                    NavManager.NavigateTo("/ScheduleMonth");
                    break;
                case "13":
                    NavManager.NavigateTo("/UsersList");
                    break;
                case "14":
                    NavManager.NavigateTo("/TeamsList");
                    break;
                default:
                    break;
            }

            SidebarToggle = false;
        }
    }
}
