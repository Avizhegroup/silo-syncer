using SiloSync.Shared.Contracts;

namespace SiloSync.Api.Services;

public interface IVehicleTagReportService
{
    Task<PagedResult<VehicleTagReportDto>> SearchAsync(VehicleTagReportQuery query, CancellationToken cancellationToken = default);

    /// <summary>Distinct, non-empty مرکز پذیرش values, sorted, for populating a filter dropdown.</summary>
    Task<IReadOnlyList<string>> GetReceptionCentersAsync(CancellationToken cancellationToken = default);

    /// <summary>Distinct, non-empty عنوان صف خودرو values, sorted, for populating a filter dropdown.</summary>
    Task<IReadOnlyList<string>> GetQueueTitlesAsync(CancellationToken cancellationToken = default);
}
