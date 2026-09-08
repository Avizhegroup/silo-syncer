using System.Net.Mime;
using Microsoft.Extensions.Options;
using SiloSync.Api.Options;
using SiloSync.Shared.Contracts;

namespace SiloSync.Api.Services;

public sealed class FileStorageService : IFileStorageService
{
    private readonly FileStorageOptions _options;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IOptions<FileStorageOptions> options, ILogger<FileStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<UploadFileResponse> SaveFileAsync(string usageId, string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(usageId))
        {
            return Failed("Usage id is required.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Failed("File name is required.");
        }

        var safeUsageId = SanitizeFileName(usageId);
        var safeFileName = SanitizeFileName(fileName);

        if (string.IsNullOrWhiteSpace(safeUsageId) || string.IsNullOrWhiteSpace(safeFileName))
        {
            return Failed("Invalid usage id or file name after sanitization.");
        }

        var directory = Path.Combine(_options.RootPath, safeUsageId);
        Directory.CreateDirectory(directory);

        var targetPath = Path.Combine(directory, safeFileName);

        if (File.Exists(targetPath))
        {
            var fileNameOnly = Path.GetFileNameWithoutExtension(safeFileName);
            var extension = Path.GetExtension(safeFileName);
            var uniqueName = $"{fileNameOnly}_{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}{extension}";
            targetPath = Path.Combine(directory, uniqueName);
            safeFileName = uniqueName;
        }

        await using var fileStream = File.Create(targetPath);
        await content.CopyToAsync(fileStream, cancellationToken);

        var url = BuildUrl(safeUsageId, safeFileName);

        _logger.LogInformation("Saved file {FileName} for usage {UsageId} at {Path}", safeFileName, safeUsageId, targetPath);

        return new UploadFileResponse
        {
            Succeeded = true,
            UsageId = safeUsageId,
            FileName = safeFileName,
            Url = url
        };
    }

    public IReadOnlyList<FileLinkDto> GetFileLinksByUsageId(string usageId)
    {
        var safeUsageId = SanitizeFileName(usageId);
        var directory = Path.Combine(_options.RootPath, safeUsageId);

        if (!Directory.Exists(directory))
        {
            return Array.Empty<FileLinkDto>();
        }

        var files = Directory.EnumerateFiles(directory)
            .Select(path => Path.GetFileName(path))
            .Where(name => !string.IsNullOrEmpty(name))
            .Select(name => new FileLinkDto
            {
                UsageId = safeUsageId,
                FileName = name!,
                Url = BuildUrl(safeUsageId, name!)
            })
            .ToList();

        return files;
    }

    public Task<Stream?> GetFileStreamAsync(string usageId, string fileName, CancellationToken cancellationToken = default)
    {
        var safeUsageId = SanitizeFileName(usageId);
        var safeFileName = SanitizeFileName(fileName);
        var filePath = Path.Combine(_options.RootPath, safeUsageId, safeFileName);

        if (!File.Exists(filePath))
        {
            return Task.FromResult<Stream?>(null);
        }

        return Task.FromResult<Stream?>(File.OpenRead(filePath));
    }

    public string? GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".mp4" => "video/mp4",
            ".mov" => "video/quicktime",
            ".avi" => "video/x-msvideo",
            ".wmv" => "video/x-ms-wmv",
            ".mkv" => "video/x-matroska",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".pdf" => "application/pdf",
            ".txt" => MediaTypeNames.Text.Plain,
            ".csv" => "text/csv",
            ".json" => MediaTypeNames.Application.Json,
            ".xml" => MediaTypeNames.Application.Xml,
            _ => MediaTypeNames.Application.Octet
        };
    }

    private string BuildUrl(string usageId, string fileName)
    {
        var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
        return $"{baseUrl}/{Uri.EscapeDataString(usageId)}/{Uri.EscapeDataString(fileName)}";
    }

    private static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(name
            .Replace('/', '_')
            .Replace('\\', '_')
            .Where(c => !invalidChars.Contains(c))
            .ToArray());

        return sanitized.Trim();
    }

    private UploadFileResponse Failed(string error)
    {
        _logger.LogWarning("File save failed: {Error}", error);
        return new UploadFileResponse
        {
            Succeeded = false,
            Error = error
        };
    }
}
