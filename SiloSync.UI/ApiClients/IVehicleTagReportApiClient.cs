using SiloSync.Shared.Contracts;

namespace SiloSync.UI.ApiClients;

public interface IVehicleTagReportApiClient
{
    Task<PagedResult<VehicleTagReportDto>> SearchAsync(VehicleTagReportQuery query, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetReceptionCentersAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetQueueTitlesAsync(CancellationToken cancellationToken = default);
}
