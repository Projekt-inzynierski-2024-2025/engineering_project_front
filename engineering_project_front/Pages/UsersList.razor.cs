namespace engineering_project_front.Pages
{
    using Microsoft.AspNetCore.Components;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::engineering_project_front.Models;
    using global::engineering_project_front.Services.Interfaces;

    namespace engineering_project_front.Pages
    {
        public partial class UsersList: ComponentBase
        {
            [Inject]
            private IUsersService UsersService { get; set; } = default!;

            private List<UsersResponse> Users { get; set; } = new();
            private List<UsersResponse> FilteredUsers { get; set; } = new();

            private string SearchTerm { get; set; } = string.Empty;

            protected override async Task OnInitializedAsync()
            {
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
                        user.LastName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
        }
    }
}
