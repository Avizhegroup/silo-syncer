using Microsoft.EntityFrameworkCore;

namespace SiloSync.Service.Data.Gallery;

public sealed class GalleryDbContext : DbContext
{
    public GalleryDbContext(DbContextOptions<GalleryDbContext> options) : base(options)
    {
    }

    public DbSet<GalleryEntity> TblGallery => Set<GalleryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GalleryEntity>(entity =>
        {
            entity.ToTable("tbl_Gallery", "dbo");
            entity.HasKey(e => e.FldGalleryId);
            entity.Property(e => e.FldGalleryId).HasColumnName("fld_GalleryId");
            entity.Property(e => e.FldGalleryUserId).HasColumnName("fld_GalleryUserId").HasMaxLength(128);
            entity.Property(e => e.FldGalleryMediaName).HasColumnName("fld_GalleryMediaName").HasMaxLength(128);
            entity.Property(e => e.FldGalleryMediaPath).HasColumnName("fld_GalleryMediaPath").HasMaxLength(512);
            entity.Property(e => e.FldGalleryUsageType).HasColumnName("fld_GalleryUsageType");
            entity.Property(e => e.FldGalleryUploadDateTime).HasColumnName("fld_GalleryUploadDateTime");
            entity.Property(e => e.FldGalleryUsageId).HasColumnName("fld_GalleryUsageId").HasMaxLength(128);
            entity.Property(e => e.FldGalleryMediaExtensionType).HasColumnName("fld_GalleryMediaExtensionType");
            entity.Property(e => e.FldGalleryAdditionalData).HasColumnName("fld_GalleryAdditionalData");
        });
    }
}
