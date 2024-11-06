using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.DropDowns;

namespace engineering_project_front.Pages
{
    public partial class AddEditTeam : ComponentBase
    {
        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        [Inject]
        private IUsersService UsersService { get; set; }
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        [Parameter]
        public long? TeamId { get; set; }

        private TeamRequest Team { get; set; } = new TeamRequest();
        private bool IsEditing => TeamId.HasValue;
        private List<UsersResponse> Managers { get; set; } = new List<UsersResponse>();


        protected override async Task OnInitializedAsync()
        {
            CreateTree();
            Managers = await UsersService.GetMenegers();

            if (IsEditing)
            {
                var response = await TeamsService.GetTeam((long)TeamId) ?? new TeamsResponse();
                if (response != null)
                {
                    MapResponseToRequest(response);
                }
            }


        }

        private void MapResponseToRequest(TeamsResponse res)
        {

            Team.ID = res.ID;
            Team.Name = res.Name;
            Team.ManagerID = res.ManagerID;


        }

        private async Task HandleValidSubmit()
        {
            if (IsEditing)
            {
                await TeamsService.EditTeam(Team);
            }
            else
            {
                await TeamsService.AddTeam(Team);
            }
            NavManager.NavigateTo("/TeamsList");
        }

        private void Cancel()
        {
            NavManager.NavigateTo("/TeamsList");
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
