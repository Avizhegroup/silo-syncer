using SiloSync.Service.Data.Gallery;
using SiloSync.Shared.Contracts;

namespace SiloSync.Service.Serivces;

public interface IFileUploader
{
    Task<UploadFileResponse> UploadAsync(GalleryEntity galleryEntity, Stream fileStream, CancellationToken cancellationToken = default);
}

internal sealed class ApiFileUploader : IFileUploader
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiFileUploader> _logger;

    public ApiFileUploader(HttpClient httpClient, ILogger<ApiFileUploader> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UploadFileResponse> UploadAsync(GalleryEntity galleryEntity, Stream fileStream, CancellationToken cancellationToken = default)
    {
        var fileName = galleryEntity.FldGalleryMediaName ?? Path.GetFileName(galleryEntity.FldGalleryMediaPath) ?? "file";

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(galleryEntity.FldGalleryUsageId ?? string.Empty), "usageId");
        content.Add(new StringContent(fileName), "mediaName");
        content.Add(new StringContent(galleryEntity.FldGalleryMediaPath ?? string.Empty), "mediaPath");
        content.Add(new StringContent(galleryEntity.FldGalleryUsageType?.ToString() ?? string.Empty), "usageType");
        content.Add(new StringContent(galleryEntity.FldGalleryUploadDateTime?.ToString("O") ?? string.Empty), "uploadDateTime");
        content.Add(new StringContent(galleryEntity.FldGalleryMediaExtensionType?.ToString() ?? string.Empty), "mediaExtensionType");
        content.Add(new StringContent(galleryEntity.FldGalleryAdditionalData ?? string.Empty), "additionalData");
        content.Add(new StringContent(galleryEntity.FldGalleryId.ToString()), "galleryId");
        content.Add(new StreamContent(fileStream), "file", fileName);

        try
        {
            var response = await _httpClient.PostAsync(string.Empty, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<UploadFileResponse>(cancellationToken);
            return result ?? new UploadFileResponse
            {
                Succeeded = response.IsSuccessStatusCode,
                UsageId = galleryEntity.FldGalleryUsageId,
                FileName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload gallery file {GalleryId} {FileName}", galleryEntity.FldGalleryId, fileName);
            return new UploadFileResponse
            {
                Succeeded = false,
                UsageId = galleryEntity.FldGalleryUsageId,
                FileName = fileName,
                Error = ex.Message
            };
        }
    }
}
