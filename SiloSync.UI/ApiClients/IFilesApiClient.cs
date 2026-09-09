using SiloSync.Shared.Contracts;

namespace SiloSync.UI.ApiClients;

public interface IFilesApiClient
{
    /// <summary>Files previously uploaded for the given usageId (i.e. سریال).</summary>
    Task<IReadOnlyList<FileLinkDto>> GetFileLinksAsync(string usageId, CancellationToken cancellationToken = default);
}
