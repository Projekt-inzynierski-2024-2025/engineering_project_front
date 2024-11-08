using Microsoft.AspNetCore.Components;

namespace engineering_project_front.Layout
{
    public partial class ChangePasswordButton
    {
        [Inject]
        private NavigationManager navManager { get; set; } = default!;

        private void onChangePasswordClicked() => navManager.NavigateTo("/reset-password");

    }
}
