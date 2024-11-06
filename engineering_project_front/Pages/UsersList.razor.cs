  using Microsoft.AspNetCore.Components;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::engineering_project_front.Models;
    using global::engineering_project_front.Services.Interfaces;
    using global::engineering_project_front.Layout;
using Syncfusion.Blazor.Grids;

    namespace engineering_project_front.Pages
    {
        public partial class UsersList: ComponentBase
        {

        [Inject]
            private NavigationManager NavManager { get; set; } = default!;

        [Inject]
            private IUsersService UsersService { get; set; } = default!;

            private List<UsersResponse> Users { get; set; } = new();
            private List<UsersResponse> FilteredUsers { get; set; } = new();

            private string SearchTerm { get; set; } = string.Empty;

            protected override async Task OnInitializedAsync()
            {
                CreateTree();
                Users = await UsersService.GetUsersAsync();
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
                    user.LastName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }


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

        private void AddUser()
        {

            NavManager.NavigateTo($"/add-edit-user/");

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

