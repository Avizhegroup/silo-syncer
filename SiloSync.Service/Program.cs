using Serilog;
using SiloSync.Service.Data.Local;

namespace SiloSync.Service;
public static partial class Program
{
    static async Task Main(string[] args)
    {
        var webApplicationOptions = new WebApplicationOptions()
        {
            ContentRootPath = AppContext.BaseDirectory,
            Args = args,
            ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName
        };

        var builder = WebApplication.CreateBuilder(webApplicationOptions);

        builder.Host
               .UseWindowsService(config =>
               {
                   config.ServiceName = "Silo Syncing Service";
               })
               .ConfigureServices((context, services) =>
               {
                   services.AddSiloSyncServices(context.Configuration);
               })
               .UseSerilog();


        var app = builder.Build();

        await SyncDatabaseInitializer.InitializeAsync(app.Services);

        app.Run();
    }
}
