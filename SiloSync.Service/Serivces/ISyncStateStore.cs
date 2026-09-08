using Microsoft.EntityFrameworkCore;
using SiloSync.Service.Data.Local;

namespace SiloSync.Service.Serivces;

public interface ISyncStateStore
{
    Task<long> GetLastGalleryIdAsync(CancellationToken cancellationToken = default);

    Task UpdateWatermarkAsync(long galleryId, CancellationToken cancellationToken = default);

    Task<bool> IsSentAsync(long galleryId, CancellationToken cancellationToken = default);

    Task MarkSentAsync(long galleryId, string? usageId, string? mediaName, string? filePath, string? remoteUrl, CancellationToken cancellationToken = default);
}

internal sealed class SyncStateStore : ISyncStateStore
{
    private readonly SyncDbContext _db;

    public SyncStateStore(SyncDbContext db)
    {
        _db = db;
    }

    public async Task<long> GetLastGalleryIdAsync(CancellationToken cancellationToken = default)
    {
        var state = await _db.SyncStates
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return state?.LastGalleryId ?? 0;
    }

    public async Task UpdateWatermarkAsync(long galleryId, CancellationToken cancellationToken = default)
    {
        var state = await _db.SyncStates
            .OrderBy(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (state is null)
        {
            state = new SyncStateEntity
            {
                Id = 1,
                LastGalleryId = galleryId,
                UpdatedAtUtc = DateTime.UtcNow
            };
            _db.SyncStates.Add(state);
        }
        else
        {
            if (galleryId > state.LastGalleryId)
            {
                state.LastGalleryId = galleryId;
                state.UpdatedAtUtc = DateTime.UtcNow;
                _db.SyncStates.Update(state);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsSentAsync(long galleryId, CancellationToken cancellationToken = default)
    {
        return await _db.SentFiles
            .AsNoTracking()
            .AnyAsync(s => s.GalleryId == galleryId, cancellationToken);
    }

    public async Task MarkSentAsync(long galleryId, string? usageId, string? mediaName, string? filePath, string? remoteUrl, CancellationToken cancellationToken = default)
    {
        var exists = await _db.SentFiles
            .AsNoTracking()
            .AnyAsync(s => s.GalleryId == galleryId, cancellationToken);

        if (exists)
        {
            return;
        }

        var sentFile = new SentFileEntity
        {
            GalleryId = galleryId,
            UsageId = usageId,
            MediaName = mediaName,
            FilePath = filePath,
            SentAtUtc = DateTime.UtcNow,
            RemoteUrl = remoteUrl
        };

        _db.SentFiles.Add(sentFile);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
