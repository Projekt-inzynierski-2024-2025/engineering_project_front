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
        #endregion

        public static SidebarMenu Instance { get; private set; } = default!;

        private bool SidebarToggle = false;

        public List<TreeData> TreeData { get; set; } = new();


        protected async override Task OnInitializedAsync()
        {
            Instance = this;

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

        public void OnSelect(NodeSelectEventArgs args)
        {
            switch (args.NodeData.Id)
            {
                case "2":
                    NavManager.NavigateTo("/");
                    break;
                case "3":
                    NavManager.NavigateTo("/login");
                    break;
                default:
                    break;
            }

            SidebarToggle = false;
        }
    }
}
