using Microsoft.EntityFrameworkCore;
using SiloSync.Api.Data.VehicleTagReport;
using SiloSync.Shared.Contracts;

namespace SiloSync.Api.Services;

public sealed class VehicleTagReportService : IVehicleTagReportService
{
    private const int DefaultPageSize = 25;
    private const int MaxPageSize = 200;

    private readonly VehicleTagReportDbContext _dbContext;
    private readonly ILogger<VehicleTagReportService> _logger;

    public VehicleTagReportService(VehicleTagReportDbContext dbContext, ILogger<VehicleTagReportService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PagedResult<VehicleTagReportDto>> SearchAsync(VehicleTagReportQuery query, CancellationToken cancellationToken = default)
    {
        var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
        var pageSize = query.PageSize is < 1 or > MaxPageSize ? DefaultPageSize : query.PageSize;

        var reportQuery = _dbContext.VehicleTagReports.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SerialNumber))
        {
            reportQuery = reportQuery.Where(r => r.SerialNumber != null && r.SerialNumber.Contains(query.SerialNumber));
        }

        if (!string.IsNullOrWhiteSpace(query.QueueTitle))
        {
            reportQuery = reportQuery.Where(r => r.QueueTitle != null && r.QueueTitle.Contains(query.QueueTitle));
        }

        if (!string.IsNullOrWhiteSpace(query.ReceptionCenter))
        {
            reportQuery = reportQuery.Where(r => r.ReceptionCenter == query.ReceptionCenter);
        }

        var totalCount = await reportQuery.CountAsync(cancellationToken);

        var entities = await reportQuery
            .OrderByDescending(r => r.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        _logger.LogDebug(
            "VehicleTagReport search returned {Count} of {TotalCount} row(s) for page {PageNumber} (size {PageSize}).",
            entities.Count, totalCount, pageNumber, pageSize);

        return new PagedResult<VehicleTagReportDto>
        {
            Items = entities.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<IReadOnlyList<string>> GetReceptionCentersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.VehicleTagReports
            .AsNoTracking()
            .Where(r => r.ReceptionCenter != null && r.ReceptionCenter != "")
            .Select(r => r.ReceptionCenter!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetQueueTitlesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.VehicleTagReports
            .AsNoTracking()
            .Where(r => r.QueueTitle != null && r.QueueTitle != "")
            .Select(r => r.QueueTitle!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    private static VehicleTagReportDto MapToDto(VehicleTagReportEntity entity) => new()
    {
        Id = entity.Id,
        SerialNumber = entity.SerialNumber,
        ShamsiDate = entity.ShamsiDate,
        PlateNumber = entity.PlateNumber,
        ChassisNumber = entity.ChassisNumber,
        EngineNumber = entity.EngineNumber,
        Vin = entity.Vin,
        VehicleModelTitle = entity.VehicleModelTitle,
        UsageTypeTitle = entity.UsageTypeTitle,
        VehicleSystemTitle = entity.VehicleSystemTitle,
        VehicleType = entity.VehicleType,
        VehicleAxle = entity.VehicleAxle,
        FuelType = entity.FuelType,
        ManufactureYear = entity.ManufactureYear,
        VehicleColor = entity.VehicleColor,
        VehicleWeight = entity.VehicleWeight,
        PlateStatus = entity.PlateStatus,
        CabinPlacard = entity.CabinPlacard,
        QueueTitle = entity.QueueTitle,
        ReceptionUser = entity.ReceptionUser,
        DocumentApprovalUser = entity.DocumentApprovalUser,
        DocumentApprovalDate = entity.DocumentApprovalDate,
        DocumentApprovalGeoCoordinates = entity.DocumentApprovalGeoCoordinates,
        FirstTrafficPoliceApprovalUser = entity.FirstTrafficPoliceApprovalUser,
        FirstApprovalDate = entity.FirstApprovalDate,
        TrafficPoliceApprovalGeoCoordinates = entity.TrafficPoliceApprovalGeoCoordinates,
        PressApprovalTrafficPoliceUser = entity.PressApprovalTrafficPoliceUser,
        PressApprovalDate = entity.PressApprovalDate,
        PressApprovalGeoCoordinates = entity.PressApprovalGeoCoordinates,
        ReceptionCenter = entity.ReceptionCenter
    };
}
