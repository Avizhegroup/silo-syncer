namespace SiloSync.Api.Data.VehicleTagReport;

/// <summary>
/// Maps to [dbo].[VehicleTagReport]. Column names in the underlying table are in Persian;
/// see <see cref="VehicleTagReportDbContext.OnModelCreating"/> for the column mapping.
/// </summary>
public sealed class VehicleTagReportEntity
{
    public int Id { get; set; }

    public string? SerialNumber { get; set; }

    public string? ShamsiDate { get; set; }

    public string? PlateNumber { get; set; }

    public string? ChassisNumber { get; set; }

    public string? EngineNumber { get; set; }

    public string? Vin { get; set; }

    public string? VehicleModelTitle { get; set; }

    public string? UsageTypeTitle { get; set; }

    public string? VehicleSystemTitle { get; set; }

    public string? VehicleType { get; set; }

    public string? VehicleAxle { get; set; }

    public string? FuelType { get; set; }

    public string? ManufactureYear { get; set; }

    public string? VehicleColor { get; set; }

    public string? VehicleWeight { get; set; }

    public string? PlateStatus { get; set; }

    public string? CabinPlacard { get; set; }

    public string? QueueTitle { get; set; }

    public string? ReceptionUser { get; set; }

    public string? DocumentApprovalUser { get; set; }

    public string? DocumentApprovalDate { get; set; }

    public string? DocumentApprovalGeoCoordinates { get; set; }

    public string? FirstTrafficPoliceApprovalUser { get; set; }

    public string? FirstApprovalDate { get; set; }

    public string? TrafficPoliceApprovalGeoCoordinates { get; set; }

    public string? PressApprovalTrafficPoliceUser { get; set; }

    public string? PressApprovalDate { get; set; }

    public string? PressApprovalGeoCoordinates { get; set; }

    public string? ReceptionCenter { get; set; }

    public string? ReceptionCenterLocation { get; set; }
}
