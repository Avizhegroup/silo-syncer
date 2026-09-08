namespace SiloSync.Service.Data.Local;

public sealed class SentFileEntity
{
    public long Id { get; set; }

    public long GalleryId { get; set; }

    public string? UsageId { get; set; }

    public string? MediaName { get; set; }

    public string? FilePath { get; set; }

    public DateTime SentAtUtc { get; set; }

    public string? RemoteUrl { get; set; }
}
