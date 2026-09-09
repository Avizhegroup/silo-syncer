using System.ComponentModel.DataAnnotations;

namespace SiloSync.Api.Options;

public sealed class VehicleTagReportOptions
{
    public const string SectionName = "VehicleTagReport";

    /// <summary>
    /// Connection string to the SQL Server database hosting [dbo].[VehicleTagReport].
    /// This is intentionally a separate connection from any other SiloSync data store —
    /// the Api only ever reads from it.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}
