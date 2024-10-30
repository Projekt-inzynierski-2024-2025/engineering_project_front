using engineering_project_front;
using engineering_project_front.Services;
using engineering_project_front.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Program
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddHttpClient("engineering-project", options => options.BaseAddress = new Uri("https://localhost:7059/"));

            builder.Services.AddScoped<ITestService, TestService>();

            await builder.Build().RunAsync();
        }
    }
}