namespace SiloSync.Shared.Contracts;

public sealed record UploadFileRequest
{
    public required string UsageId { get; init; }

    public string? MediaName { get; init; }

    public string? MediaPath { get; init; }

    public int? UsageType { get; init; }

    public DateTime? UploadDateTime { get; init; }

    public int? MediaExtensionType { get; init; }

    public string? AdditionalData { get; init; }

    public long? GalleryId { get; init; }

    public Stream? FileContent { get; init; }

    public string? FileName { get; init; }
}
