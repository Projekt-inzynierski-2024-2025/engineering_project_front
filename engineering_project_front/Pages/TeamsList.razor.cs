using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class TeamsList: ComponentBase
    {
        #region Injection
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        #endregion


        private List<TeamsResponse> Teams { get; set; } = new();
        private List<TeamsResponse> FilteredTeams { get; set; } = new();

        private string SearchTerm { get; set; } = string.Empty;

        #region ToastAndNotification
        private SfToast? Toast;
        private string Message { get; set; } = string.Empty;
        private string Title { get; set; } = string.Empty;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            CreateTree();

            var response = await TeamsService.GetTeamsAsync();
            if (response.Success)
            {
                Teams = response.Data;
                FilteredTeams = Teams;
            }
            else
            {
                ShowToast(response.Message, response.Success);
            }

            FilteredTeams = Teams;
        }

        #region ToastAndNotification
        private async Task ShowToast(string message, bool success )
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


        private void FilterTeams()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredTeams = Teams;
            }
            else
            {
                FilteredTeams = Teams.Where(team =>
                    team.ManagerName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    team.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        private void OnContextMenuClick(ContextMenuClickEventArgs<TeamsResponse> args)
        {
            if (args == null)
                return;

            switch (args.Item.Id)
            {
                case "seeDetails":
                    NavManager.NavigateTo($"/TeamDetails/{args.RowInfo.RowData.ID}");
                    break;
                default:
                    break;
            }
        }

        private void AddTeam()
        {
            NavManager.NavigateTo($"/add-edit-team/");
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
