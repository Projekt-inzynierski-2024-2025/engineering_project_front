using Syncfusion.Blazor;
using System.Resources;

namespace engineering_project_front.Services
{
    public class Localizer : ISyncfusionStringLocalizer
    {
        public ResourceManager ResourceManager => Resources.SfResources.ResourceManager;

        public string? GetText(string key)
        {
            return ResourceManager.GetString(key);
        }
    }
}
