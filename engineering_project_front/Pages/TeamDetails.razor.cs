using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Xml.Serialization;

namespace engineering_project_front.Pages
{
    public partial class TeamDetails : ComponentBase
    {
        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Parameter]
        public required string ParamID { get; set; }

        private long ID => long.Parse(ParamID);
        private TeamsResponse? Team { get; set; } = new TeamsResponse();

        protected override async Task OnInitializedAsync()
        {
            await GetTeam();
            CreateTree();
            await base.OnInitializedAsync();
        }

        private async Task GetTeam()
        {
            Team = await TeamsService.GetTeam(ID);
        }

        private void EditTeam()
        {
            NavManager.NavigateTo($"/add-edit-team/{Team.ID}");
        }

        private void DeleteTeam()
        {
            TeamsService.DeleteTeam(ID);
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
                }
            };
        }
    }
}
