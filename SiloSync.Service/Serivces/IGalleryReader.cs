using Microsoft.EntityFrameworkCore;
using SiloSync.Service.Data.Gallery;
using SiloSync.Service.Data.Local;

namespace SiloSync.Service.Serivces;

public interface IGalleryReader
{
    Task<IReadOnlyList<GalleryEntity>> GetNewFilesAsync(long afterId, int batchSize, CancellationToken cancellationToken = default);
}

internal sealed class GalleryReader : IGalleryReader
{
    private readonly GalleryDbContext _galleryDb;
    private readonly SyncDbContext _syncDb;

    public GalleryReader(GalleryDbContext galleryDb, SyncDbContext syncDb)
    {
        _galleryDb = galleryDb;
        _syncDb = syncDb;
    }

    public async Task<IReadOnlyList<GalleryEntity>> GetNewFilesAsync(long afterId, int batchSize, CancellationToken cancellationToken = default)
    {
        _ = _syncDb; // ensures the local db is initialized in the DI scope if needed
        var rows = await _galleryDb.TblGallery
            .AsNoTracking()
            .Where(g => g.FldGalleryId > afterId)
            .OrderBy(g => g.FldGalleryId)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        return rows;
    }
}
