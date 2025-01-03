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
        private string? LastSelectedId { get; set; }


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
                    Id = "5",
                    Pid = "1",
                    Name = "Moje konto"
                },
                new TreeData()
                {
                    Id = "6",
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
                        Id = "4",
                        Name = "Panel Administracyjny",
                        HasChild = true,
                        Expanded = true,
                    },
                    new TreeData
                    {
                        Id = "14",
                        Pid = "4",
                        Name = "Zarządzanie użytkownikami",
                    },
                    new TreeData
                    {
                        Id = "15",
                        Pid = "4",
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
                        Id = "2",
                        Name = "Panel Pracownika",
                        HasChild = true,
                        Expanded = true,
                    },
                    new TreeData
                    {
                        Id = "7",
                        Pid = "2",
                        Name = "Strona głowna",
                    },
                    new TreeData()
                    {
                        Id= "8",
                        Pid = "2",
                        Name = "Zmień godziny pracy"
                    },
                    new TreeData()
                    {
                        Id = "9",
                        Pid = "2",
                        Name = "Dyspozycja"
                    },
                    new TreeData()
                    {
                        Id = "10",
                        Pid = "2",
                        Name = "Czas pracy"
                    },
                    new TreeData()
                    {
                        Id = "11",
                        Pid = "2",
                        Name = "Moje zmiany"
                    },
                });

                //For Manager
                if (Role == "Kierownik")
                {
                    TreeData.AddRange(new List<TreeData>{
                    new TreeData
                    {
                        Id = "3",
                        Name = "Panel Kierownika",
                        HasChild = true,
                        Expanded = true,
                    },
                    new TreeData
                    {
                        Id = "12",
                        Pid = "3",
                        Name = "Moi Pracownicy",
                    },
                    new TreeData
                    {
                        Id = "13",
                        Pid = "3",
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
                case "5":
                    NavManager.NavigateTo("/my-account");
                    break;
                case "6":
                    sessionStorage.RemoveItemAsync("token");
                    sessionStorage.RemoveItemAsync("role");
                    NavManager.NavigateTo("/");
                    break;
                case "7":
                    NavManager.NavigateTo("/home");
                    break;
                case "8":
                    NavManager.NavigateTo("/edit-work");
                    break;
                case "9":
                    NavManager.NavigateTo("/availability-scheduler");
                    break;
                case "10":
                    NavManager.NavigateTo("/show-time");
                    break;
                case "11":
                    NavManager.NavigateTo("/myShifts");
                    break;
                case "12":
                    NavManager.NavigateTo("/MyEmployees");
                    break;
                case "13":
                    NavManager.NavigateTo("/ScheduleMonth");
                    break;
                case "14":
                    NavManager.NavigateTo("/UsersList");
                    break;
                case "15":
                    NavManager.NavigateTo("/TeamsList");
                    break;
                default:
                    break;
            }

            SidebarToggle = false;
        }

        private void OnNodeClicked(NodeClickEventArgs args)
        {
            var clickedNode = TreeData.FirstOrDefault(node => node.Id == args.NodeData.Id);

            if (clickedNode == null)
                return;

            if (LastSelectedId == clickedNode.Id)
            {
                
                NavigateToNode(clickedNode.Id);
                SidebarToggle = false;
            }
            else
            {
                
                LastSelectedId = clickedNode.Id;

                foreach (var node in TreeData)
                {
                    node.Selected = node.Id == clickedNode.Id;
                }

                
                NavigateToNode(clickedNode.Id);
                SidebarToggle = false;
            }
        }

        private void NavigateToNode(string nodeId)
        {
            switch (nodeId)
            {
                case "5":
                    NavManager.NavigateTo("/my-account");
                    break;
                case "6":
                    sessionStorage.RemoveItemAsync("token");
                    sessionStorage.RemoveItemAsync("role");
                    NavManager.NavigateTo("/");
                    break;
                case "7":
                    NavManager.NavigateTo("/home");
                    break;
                case "8":
                    NavManager.NavigateTo("/edit-work");
                    break;
                case "9":
                    NavManager.NavigateTo("/availability-scheduler");
                    break;
                case "10":
                    NavManager.NavigateTo("/show-time");
                    break;
                case "11":
                    NavManager.NavigateTo("/myShifts");
                    break;
                case "12":
                    NavManager.NavigateTo("/MyEmployees");
                    break;
                case "13":
                    NavManager.NavigateTo("/ScheduleMonth");
                    break;
                case "14":
                    NavManager.NavigateTo("/UsersList");
                    break;
                case "15":
                    NavManager.NavigateTo("/TeamsList");
                    break;
                default:
                    break;
            }
        }
    }
}
