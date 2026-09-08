namespace SiloSync.Service.Data.Gallery;

public sealed class GalleryEntity
{
    public long FldGalleryId { get; set; }

    public string? FldGalleryUserId { get; set; }

    public string? FldGalleryMediaName { get; set; }

    public string? FldGalleryMediaPath { get; set; }

    public int? FldGalleryUsageType { get; set; }

    public DateTime? FldGalleryUploadDateTime { get; set; }

    public string? FldGalleryUsageId { get; set; }

    public int? FldGalleryMediaExtensionType { get; set; }

    public string? FldGalleryAdditionalData { get; set; }
}
