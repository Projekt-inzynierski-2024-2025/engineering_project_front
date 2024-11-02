using engineering_project_front;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;

namespace engineering_project_front
{
    public class Program
    {
        //Temporary, I don't like it here but I don't know where to put this for now
        private const string SYNCFUSION_KEY = "Ngo9BigBOggjHTQxAR8/V1NCaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXhfcXRdRGRfUUR0Wks=";

        public static async Task Main(string[] args)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_KEY);

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddHttpClient("engineering-project", options => options.BaseAddress = new Uri("https://localhost:7059/"));

            
            builder.Services.AddScoped<IUsersService, UsersService>();

            builder.Services.AddScoped<SfDialogService>();

            builder.Services.AddSyncfusionBlazor();

            await builder.Build().RunAsync();
        }
    }
}