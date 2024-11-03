using engineering_project_front.Layout;
using engineering_project_front.Models;

namespace engineering_project_front.Pages
{
    public partial class Login
    {
        protected async override Task OnInitializedAsync()
        {
            CreateTree();

            await base.OnInitializedAsync();
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
                    Selected = true
                }
            ];
        }
    }
}
