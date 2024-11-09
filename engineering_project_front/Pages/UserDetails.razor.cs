using engineering_project_front.Layout;
using engineering_project_front.Models;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;

namespace engineering_project_front.Pages
{
    public partial class UserDetails : ComponentBase
    {
        #region Injection

        [Inject]
        private IUsersService UsersService { get; set; } = default!;
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;
        #endregion


        [Parameter]
        public required string ParamID { get; set; }

        private long ID => long.Parse(ParamID);
        private UsersResponse? User { get; set; } = new UsersResponse();

        #region ToastAndNotification
        private SfToast? Toast;
        private string Message { get; set; } = string.Empty;
        private bool IsDeleteDialogVisible { get; set; } = false;
        #endregion

        protected async override Task OnInitializedAsync()
        {


            await GetUser();

            CreateTree();

            await base.OnInitializedAsync();
        }

        private async Task GetUser()
        {
             
            var response= await UsersService.GetUser(ID);

            if (response.Success)
            {
                User = response.Data;

            }
            else
            {
                ShowToast(response.Message);
            }
        }

        #region ToastAndNotification
        private async Task ShowToast(string message)
        {
            Message = message;
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


        private void EditUser()
        {

            NavManager.NavigateTo($"/add-edit-user/{User.ID}");

        }

        private async Task ConfirmDelete()
        {
            IsDeleteDialogVisible = false;
            var response = await UsersService.DeleteUser(ID);

            if (response.Success)
            {
                User = new UsersResponse();
                ShowToast(response.Message);
                NavManager.NavigateTo("/UsersList");
            }
            else
            {
                ShowToast(response.Message);
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

                }
            };
        }
    }
}
