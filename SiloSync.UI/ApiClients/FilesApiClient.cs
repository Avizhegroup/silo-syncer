using System.Net.Http.Json;
using SiloSync.Shared.Contracts;

namespace SiloSync.UI.ApiClients;

public sealed class FilesApiClient : IFilesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FilesApiClient> _logger;

    public FilesApiClient(HttpClient httpClient, ILogger<FilesApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<FileLinkDto>> GetFileLinksAsync(string usageId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(usageId))
        {
            return Array.Empty<FileLinkDto>();
        }

        try
        {
            var links = await _httpClient.GetFromJsonAsync<List<FileLinkDto>>(
                $"api/files/{Uri.EscapeDataString(usageId)}", cancellationToken);

            return links ?? new List<FileLinkDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch files for usage id {UsageId}.", usageId);
            return Array.Empty<FileLinkDto>();
        }
    }
}
