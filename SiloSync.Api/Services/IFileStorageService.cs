using SiloSync.Shared.Contracts;

namespace SiloSync.Api.Services;

public interface IFileStorageService
{
    Task<UploadFileResponse> SaveFileAsync(string usageId, string fileName, Stream content, CancellationToken cancellationToken = default);

    IReadOnlyList<FileLinkDto> GetFileLinksByUsageId(string usageId);

    Task<Stream?> GetFileStreamAsync(string usageId, string fileName, CancellationToken cancellationToken = default);

    string? GetContentType(string fileName);
}
