using Microsoft.Extensions.Options;
using SiloSync.Api.Extensions;
using SiloSync.Api.Options;
using SiloSync.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 500L * 1024 * 1024;
});

builder.Services.AddOptions<FileStorageOptions>()
    .Bind(builder.Configuration.GetSection(FileStorageOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IFileStorageService, FileStorageService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSiloSerilog(builder.Configuration);

var app = builder.Build();

EnsureStorageDirectoryExists(app.Services);

app.UseSwagger();

app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void EnsureStorageDirectoryExists(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<FileStorageOptions>>().Value;

    if (!string.IsNullOrWhiteSpace(options.RootPath) && !Directory.Exists(options.RootPath))
    {
        Directory.CreateDirectory(options.RootPath);
    }
}
