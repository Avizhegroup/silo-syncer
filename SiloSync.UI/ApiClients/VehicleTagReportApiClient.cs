using System.Net.Http.Json;
using SiloSync.Shared.Contracts;

namespace SiloSync.UI.ApiClients;

public sealed class VehicleTagReportApiClient : IVehicleTagReportApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehicleTagReportApiClient> _logger;

    public VehicleTagReportApiClient(HttpClient httpClient, ILogger<VehicleTagReportApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedResult<VehicleTagReportDto>> SearchAsync(VehicleTagReportQuery query, CancellationToken cancellationToken = default)
    {
        var emptyResult = new PagedResult<VehicleTagReportDto>
        {
            Items = Array.Empty<VehicleTagReportDto>(),
            TotalCount = 0,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        try
        {
            var result = await _httpClient.GetFromJsonAsync<PagedResult<VehicleTagReportDto>>(
                $"api/vehicletagreport{BuildQueryString(query)}", cancellationToken);

            return result ?? emptyResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch vehicle tag report data from the Api.");
            return emptyResult;
        }
    }

    public async Task<IReadOnlyList<string>> GetReceptionCentersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var centers = await _httpClient.GetFromJsonAsync<List<string>>("api/vehicletagreport/reception-centers", cancellationToken);
            return centers ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch reception centers from the Api.");
            return Array.Empty<string>();
        }
    }

    private static string BuildQueryString(VehicleTagReportQuery query)
    {
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.SerialNumber))
        {
            parameters.Add($"serialNumber={Uri.EscapeDataString(query.SerialNumber)}");
        }

        if (!string.IsNullOrWhiteSpace(query.QueueTitle))
        {
            parameters.Add($"queueTitle={Uri.EscapeDataString(query.QueueTitle)}");
        }

        if (!string.IsNullOrWhiteSpace(query.ReceptionCenter))
        {
            parameters.Add($"receptionCenter={Uri.EscapeDataString(query.ReceptionCenter)}");
        }

        parameters.Add($"pageNumber={query.PageNumber}");
        parameters.Add($"pageSize={query.PageSize}");

        return "?" + string.Join('&', parameters);
    }
}
