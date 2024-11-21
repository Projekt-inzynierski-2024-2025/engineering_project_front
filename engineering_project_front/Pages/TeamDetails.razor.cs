using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Models.Responses;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class TeamDetails : ComponentBase
    {
        #region Injects
        [Inject]
        private ITeamsService TeamsService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        #endregion


        [Parameter]
        public required string ParamID { get; set; }

        private long ID => long.Parse(ParamID);
        private TeamsResponse? Team { get; set; } = new TeamsResponse();




        #region ToastAndNotification
        private SfToast? Toast;
        private bool IsDeleteDialogVisible { get; set; } = false;
        private string Title { get; set; } = string.Empty;
        private string Message { get; set; } = string.Empty;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            CreateTree();
            await GetTeam();
            await base.OnInitializedAsync();
        }

        private async Task GetTeam()
        {

            var response = await TeamsService.GetTeam(ID);

            if (response.Success)
            {
                Team = response.Data;
            }
            else
            {
                ShowToast(response.Message, response.Success);
            }
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
        private void ShowDeleteConfirmation()
        {
            IsDeleteDialogVisible = true;
        }

        private void CloseDeleteDialog()
        {
            IsDeleteDialogVisible = false;
        }
        #endregion


        private void EditTeam()
        {
            NavManager.NavigateTo($"/add-edit-team/{Team.ID}");
        }

        private async Task ConfirmDelete()
        {
            IsDeleteDialogVisible = false;
            var response = await TeamsService.DeleteTeam(ID);

            if (response.Success)
            {
                Team = new TeamsResponse();
                ShowToast(response.Message, response.Success);
                NavManager.NavigateTo("/TeamsList");
            }
            else
            {
                ShowToast(response.Message, response.Success);
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

                new TreeData
                {
                    Id = "6",
                    Pid = "1",
                    Name = "Grafik",
                },
                new TreeData
                {
                    Id = "7",
                    Pid = "1",
                    Name = "Moi Pracownicy",
                },
                new TreeData()
                {
                    Id= "8",
                    Pid = "1",
                    Name = "Zmień godziny pracy"
                },
                new TreeData()
                {
                    Id = "9",
                    Pid = "1",
                    Name = "Sprawdź dostępności godzinowe"
                }
            };

        }
    }
}
