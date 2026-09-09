namespace SiloSync.Shared.Contracts;

/// <summary>
/// Filter + paging parameters for searching [dbo].[VehicleTagReport].
/// </summary>
public sealed record VehicleTagReportQuery
{
    /// <summary>Partial match against سریال (also the file-storage UsageId).</summary>
    public string? SerialNumber { get; init; }

    /// <summary>Partial match against عنوان صف خودرو.</summary>
    public string? QueueTitle { get; init; }

    /// <summary>Exact match against مرکز پذیرش.</summary>
    public string? ReceptionCenter { get; init; }

    /// <summary>1-based page number.</summary>
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 25;
}
