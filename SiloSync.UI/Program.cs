using MudBlazor.Services;
using SiloSync.UI.ApiClients;
using SiloSync.UI.Components;
using SiloSync.UI.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddOptions<ApiOptions>()
    .Bind(builder.Configuration.GetSection(ApiOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var apiOptions = builder.Configuration.GetSection(ApiOptions.SectionName).Get<ApiOptions>() ?? new ApiOptions();

// Both clients call SiloSync.Api from the server (Blazor Server), so no CORS setup is needed.
builder.Services.AddHttpClient<IVehicleTagReportApiClient, VehicleTagReportApiClient>(client =>
{
    client.BaseAddress = new Uri(apiOptions.BaseUrl);
});

builder.Services.AddHttpClient<IFilesApiClient, FilesApiClient>(client =>
{
    client.BaseAddress = new Uri(apiOptions.BaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
