using Microsoft.EntityFrameworkCore;

namespace SiloSync.Api.Data.VehicleTagReport;

/// <summary>
/// Read-only access to [dbo].[VehicleTagReport]. This is a separate SQL Server connection
/// from the Gallery sync database used by SiloSync.Service — the Api never writes here.
/// </summary>
public sealed class VehicleTagReportDbContext : DbContext
{
    public VehicleTagReportDbContext(DbContextOptions<VehicleTagReportDbContext> options) : base(options)
    {
    }

    public DbSet<VehicleTagReportEntity> VehicleTagReports => Set<VehicleTagReportEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VehicleTagReportEntity>(entity =>
        {
            entity.ToTable("VehicleTagReport", "dbo");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.SerialNumber).HasColumnName("سریال").HasMaxLength(200);
            entity.Property(e => e.ShamsiDate).HasColumnName("تاریخ شمسی").HasMaxLength(10);
            entity.Property(e => e.PlateNumber).HasColumnName("شماره پلاک انتظامی").HasMaxLength(100);
            entity.Property(e => e.ChassisNumber).HasColumnName("شماره شاسی").HasMaxLength(100);
            entity.Property(e => e.EngineNumber).HasColumnName("شماره موتور").HasMaxLength(100);
            entity.Property(e => e.Vin).HasColumnName("VIN").HasMaxLength(100);
            entity.Property(e => e.VehicleModelTitle).HasColumnName("عنوان مدل خودرو").HasMaxLength(500);
            entity.Property(e => e.UsageTypeTitle).HasColumnName("عنوان نوع کاربری").HasMaxLength(500);
            entity.Property(e => e.VehicleSystemTitle).HasColumnName("عنوان سیستم خودرو").HasMaxLength(500);
            entity.Property(e => e.VehicleType).HasColumnName("تیپ خودرو").HasMaxLength(200);
            entity.Property(e => e.VehicleAxle).HasColumnName("محور خودرو").HasMaxLength(100);
            entity.Property(e => e.FuelType).HasColumnName("نوع سوخت").HasMaxLength(200);
            entity.Property(e => e.ManufactureYear).HasColumnName("سال تولید").HasMaxLength(20);
            entity.Property(e => e.VehicleColor).HasColumnName("رنگ خودرو").HasMaxLength(200);
            entity.Property(e => e.VehicleWeight).HasColumnName("وزن خودرو").HasMaxLength(50);
            entity.Property(e => e.PlateStatus).HasColumnName("وضعیت پلاک").HasMaxLength(200);
            entity.Property(e => e.CabinPlacard).HasColumnName("پلاکت اتاق").HasMaxLength(200);
            entity.Property(e => e.QueueTitle).HasColumnName("عنوان صف خودرو").HasMaxLength(500);
            entity.Property(e => e.ReceptionUser).HasColumnName("کاربر پذیرش").HasMaxLength(200);
            entity.Property(e => e.DocumentApprovalUser).HasColumnName("کاربر تأیید مدارک").HasMaxLength(200);
            entity.Property(e => e.DocumentApprovalDate).HasColumnName("تاریخ تأیید مدارک").HasMaxLength(50);
            entity.Property(e => e.DocumentApprovalGeoCoordinates).HasColumnName("مختصات جغرافیایی تأیید مدارک").HasMaxLength(200);
            entity.Property(e => e.FirstTrafficPoliceApprovalUser).HasColumnName("راهور تأیید اول").HasMaxLength(200);
            entity.Property(e => e.FirstApprovalDate).HasColumnName("تاریخ تأیید اول").HasMaxLength(50);
            entity.Property(e => e.TrafficPoliceApprovalGeoCoordinates).HasColumnName("مختصات جغرافیایی تأیید راهور").HasMaxLength(200);
            entity.Property(e => e.PressApprovalTrafficPoliceUser).HasColumnName("راهور تأیید پرس").HasMaxLength(200);
            entity.Property(e => e.PressApprovalDate).HasColumnName("تاریخ تأیید پرس").HasMaxLength(50);
            entity.Property(e => e.PressApprovalGeoCoordinates).HasColumnName("مختصات جغرافیایی تأیید پرس").HasMaxLength(200);
            entity.Property(e => e.ReceptionCenter).HasColumnName("مرکز پذیرش").HasMaxLength(200);

            // Matches [UQ_VehicleTagReport_Serial] on the underlying table.
            entity.HasIndex(e => e.SerialNumber).IsUnique();
        });
    }
}
