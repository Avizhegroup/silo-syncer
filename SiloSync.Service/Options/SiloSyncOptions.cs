using System.ComponentModel.DataAnnotations;

namespace SiloSync.Service.Options;

public sealed class SiloSyncOptions
{
    public const string SectionName = "SiloSync";

    [Range(1, 1440)]
    public int PollIntervalMinutes { get; set; } = 1;

    [Range(1, 1000)]
    public int BatchSize { get; set; } = 50;

    public SqlServerSourceOptions SqlServer { get; set; } = new();

    public SqliteLocalOptions Sqlite { get; set; } = new();

    public ApiClientOptions Api { get; set; } = new();
}

public sealed class SqlServerSourceOptions
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public string MediaRootPath { get; set; } = string.Empty;
}

public sealed class SqliteLocalOptions
{
    [Required]
    public string DatabasePath { get; set; } = "data\\SiloSync.db";
}

public sealed class ApiClientOptions
{
    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;

    [Required]
    public string UploadEndpoint { get; set; } = "/api/files";
}
