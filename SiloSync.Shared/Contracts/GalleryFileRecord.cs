namespace SiloSync.Shared.Contracts;

public sealed record GalleryFileRecord
{
    public required long GalleryId { get; init; }

    public string? UserId { get; init; }

    public string? MediaName { get; init; }

    public string? MediaPath { get; init; }

    public int? UsageType { get; init; }

    public DateTime? UploadDateTime { get; init; }

    public string? UsageId { get; init; }

    public int? MediaExtensionType { get; init; }

    public string? AdditionalData { get; init; }
}
