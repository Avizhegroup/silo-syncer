namespace SiloSync.Service.Data.Local;

public sealed class SyncStateEntity
{
    public int Id { get; set; }

    public long LastGalleryId { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
