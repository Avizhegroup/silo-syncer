using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using SiloSync.Service.Data.Gallery;
using SiloSync.Service.Data.Local;
using SiloSync.Service.Options;
using SiloSync.Service.Serivces;

namespace SiloSync.Service;

public static partial class Program
{
    public static void AddSiloSyncServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSiloSerilogForWindowsServices(configuration);

        services.AddOptions<SiloSyncOptions>()
            .Bind(configuration.GetSection(SiloSyncOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var options = configuration.GetSection(SiloSyncOptions.SectionName).Get<SiloSyncOptions>() ?? new SiloSyncOptions();

        services.AddDbContext<GalleryDbContext>((_, dbBuilder) =>
        {
            dbBuilder.UseSqlServer(options.SqlServer.ConnectionString, sql =>
            {
                sql.CommandTimeout(120);
            });
        });

        var sqliteConnectionString = GetSqliteConnectionString(options.Sqlite.DatabasePath);
        services.AddDbContext<SyncDbContext>((_, dbBuilder) =>
        {
            dbBuilder.UseSqlite(sqliteConnectionString);
        });

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        services.AddHttpClient<IFileUploader, ApiFileUploader>(client =>
        {
            client.BaseAddress = new Uri(new Uri(options.Api.BaseUrl), options.Api.UploadEndpoint.TrimStart('/'));
            client.Timeout = TimeSpan.FromMinutes(5);
        }).AddPolicyHandler(retryPolicy);

        services.AddScoped<IGalleryReader, GalleryReader>();
        services.AddScoped<ISyncStateStore, SyncStateStore>();

        services.AddHostedService<Worker>();
    }

    private static string GetSqliteConnectionString(string databasePath)
    {
        var fullPath = Path.IsPathRooted(databasePath)
            ? databasePath
            : Path.Combine(AppContext.BaseDirectory, databasePath);

        return $"Data Source={fullPath}";
    }
}
