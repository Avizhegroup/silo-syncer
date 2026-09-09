namespace SiloSync.Shared.Contracts;

/// <summary>
/// Read-only projection of a row from [dbo].[VehicleTagReport].
/// Property names are English; each maps to a Persian column noted in the comments.
/// </summary>
public sealed record VehicleTagReportDto
{
    public int Id { get; init; }

    /// <summary>سریال — also used as the file-storage UsageId.</summary>
    public string? SerialNumber { get; init; }

    /// <summary>تاریخ شمسی</summary>
    public string? ShamsiDate { get; init; }

    /// <summary>شماره پلاک انتظامی</summary>
    public string? PlateNumber { get; init; }

    /// <summary>شماره شاسی</summary>
    public string? ChassisNumber { get; init; }

    /// <summary>شماره موتور</summary>
    public string? EngineNumber { get; init; }

    /// <summary>VIN</summary>
    public string? Vin { get; init; }

    /// <summary>عنوان مدل خودرو</summary>
    public string? VehicleModelTitle { get; init; }

    /// <summary>عنوان نوع کاربری</summary>
    public string? UsageTypeTitle { get; init; }

    /// <summary>عنوان سیستم خودرو</summary>
    public string? VehicleSystemTitle { get; init; }

    /// <summary>تیپ خودرو</summary>
    public string? VehicleType { get; init; }

    /// <summary>محور خودرو</summary>
    public string? VehicleAxle { get; init; }

    /// <summary>نوع سوخت</summary>
    public string? FuelType { get; init; }

    /// <summary>سال تولید</summary>
    public string? ManufactureYear { get; init; }

    /// <summary>رنگ خودرو</summary>
    public string? VehicleColor { get; init; }

    /// <summary>وزن خودرو</summary>
    public string? VehicleWeight { get; init; }

    /// <summary>وضعیت پلاک</summary>
    public string? PlateStatus { get; init; }

    /// <summary>پلاکت اتاق</summary>
    public string? CabinPlacard { get; init; }

    /// <summary>عنوان صف خودرو — "stack / queue title".</summary>
    public string? QueueTitle { get; init; }

    /// <summary>کاربر پذیرش</summary>
    public string? ReceptionUser { get; init; }

    /// <summary>کاربر تأیید مدارک</summary>
    public string? DocumentApprovalUser { get; init; }

    /// <summary>تاریخ تأیید مدارک</summary>
    public string? DocumentApprovalDate { get; init; }

    /// <summary>مختصات جغرافیایی تأیید مدارک</summary>
    public string? DocumentApprovalGeoCoordinates { get; init; }

    /// <summary>راهور تأیید اول</summary>
    public string? FirstTrafficPoliceApprovalUser { get; init; }

    /// <summary>تاریخ تأیید اول</summary>
    public string? FirstApprovalDate { get; init; }

    /// <summary>مختصات جغرافیایی تأیید راهور</summary>
    public string? TrafficPoliceApprovalGeoCoordinates { get; init; }

    /// <summary>راهور تأیید پرس</summary>
    public string? PressApprovalTrafficPoliceUser { get; init; }

    /// <summary>تاریخ تأیید پرس</summary>
    public string? PressApprovalDate { get; init; }

    /// <summary>مختصات جغرافیایی تأیید پرس</summary>
    public string? PressApprovalGeoCoordinates { get; init; }

    /// <summary>مرکز پذیرش</summary>
    public string? ReceptionCenter { get; init; }
}
