using Microsoft.EntityFrameworkCore;

namespace SiloSync.Service.Data.Local;

public sealed class SyncDbContext : DbContext
{
    public SyncDbContext(DbContextOptions<SyncDbContext> options) : base(options)
    {
    }

    public DbSet<SyncStateEntity> SyncStates => Set<SyncStateEntity>();

    public DbSet<SentFileEntity> SentFiles => Set<SentFileEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SyncStateEntity>(entity =>
        {
            entity.ToTable("SyncStates");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Id).IsUnique();
        });

        modelBuilder.Entity<SentFileEntity>(entity =>
        {
            entity.ToTable("SentFiles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.GalleryId).IsUnique();
            entity.HasIndex(e => e.UsageId);
        });
    }
}
