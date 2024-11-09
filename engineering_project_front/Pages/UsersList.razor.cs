using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using global::engineering_project_front.Models;
using global::engineering_project_front.Services.Interfaces;
using global::engineering_project_front.Layout;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using engineering_project_front.Models.Responses;

namespace engineering_project_front.Pages
{
    public partial class UsersList: ComponentBase
        {
        #region Injection

        [Inject]
            private NavigationManager NavManager { get; set; } = default!;

        [Inject]
            private IUsersService UsersService { get; set; } = default!;

        #endregion
            private List<UsersResponse> Users { get; set; } = new();
            private List<UsersResponse> FilteredUsers { get; set; } = new();
            private string SearchTerm { get; set; } = string.Empty;

        #region Toast
            private SfToast? Toast;
            private string Title { get; set; } = string.Empty;
            private string Message { get; set; } = string.Empty;
        #endregion
        protected override async Task OnInitializedAsync()
            {
                CreateTree();
                var response = await UsersService.GetUsersAsync();
                if (response.Success)
                {
                    Users = response.Data;
                    FilteredUsers = Users;
                }
                else
                {
                   
                    ShowToast(response.Message, response.Success);
                }
                FilteredUsers = Users;
        }

       
        private void FilterUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredUsers = Users;
            }
            else
            {
                FilteredUsers = Users.Where(user =>
                    user.FirstName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    user.TeamName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    user.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    user.RoleName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    user.LastName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        private void AddUser()
        {

            NavManager.NavigateTo($"/add-edit-user/");

        }

        #region ToastAndHelpers
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
        private string GetRoleText(int role)
        {
            return role switch
            {
                0 => "Administrator",
                1 => "Kierownik",
                2 => "Pracownik",
                _ => "Nieznany"
            };
        }
        #endregion
        private void OnContextMenuClick(ContextMenuClickEventArgs<UsersResponse> args)
        {
            if (args == null)
                return;

            try
            {
                switch (args.Item.Id)
                {
                    case "seeDetails":
                        NavManager.NavigateTo($"/UserDetails/{args.RowInfo.RowData.ID}");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
            }
        }

        private void CreateTree()
            {
                SidebarMenu.Instance.TreeData =
                [
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
                    Name = "Login",
                },
                new TreeData
                {
                    Id = "4",
                    Pid = "1",
                    Name = "Zarządzanie użytkownikami",
                    Selected = true
                },
                new TreeData
                {
                    Id = "5",
                    Pid = "1",
                    Name = "Zarządzanie zespołami",
                    
                }
                ];
            }
        }
    }

