using engineering_project_front;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Blazored.SessionStorage;

using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;

namespace Program
{
    public class Program
    {
        //Temporary, I don't like it here but I don't know where to put this for now
        private const string SYNCFUSION_KEY = "Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH5edHVURGBZWUV/X0s=";

        public static async Task Main(string[] args)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_KEY);

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddHttpClient("engineering-project", options => options.BaseAddress = new Uri("https://localhost:7059/"));

            builder.Services.AddScoped<IAvailabilitiesService, AvailabilitiesService>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IResetPassword, ResetPassword>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<ITeamsService, TeamsService>();
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IWorksService, WorksService>();
            builder.Services.AddScoped<IValidateRole, ValidateRole>();

            builder.Services.AddScoped<SfDialogService>();

            builder.Services.AddBlazoredSessionStorage();

            builder.Services.AddSyncfusionBlazor();

            builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(Localizer));



            await builder.Build().RunAsync();
        }
    }
}