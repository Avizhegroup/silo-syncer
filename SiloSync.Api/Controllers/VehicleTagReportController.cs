using Microsoft.AspNetCore.Mvc;
using SiloSync.Api.Services;
using SiloSync.Shared.Contracts;

namespace SiloSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VehicleTagReportController : ControllerBase
{
    private readonly IVehicleTagReportService _reportService;

    public VehicleTagReportController(IVehicleTagReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// GET /api/vehicletagreport?serialNumber=&amp;queueTitle=&amp;receptionCenter=&amp;pageNumber=1&amp;pageSize=25
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<VehicleTagReportDto>>> Search(
        [FromQuery] string? serialNumber,
        [FromQuery] string? queueTitle,
        [FromQuery] string? receptionCenter,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var query = new VehicleTagReportQuery
        {
            SerialNumber = serialNumber,
            QueueTitle = queueTitle,
            ReceptionCenter = receptionCenter,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _reportService.SearchAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>GET /api/vehicletagreport/reception-centers — distinct مرکز پذیرش values for the filter dropdown.</summary>
    [HttpGet("reception-centers")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetReceptionCenters(CancellationToken cancellationToken)
    {
        var centers = await _reportService.GetReceptionCentersAsync(cancellationToken);
        return Ok(centers);
    }

    /// <summary>GET /api/vehicletagreport/queue-titles — distinct عنوان صف خودرو values for the filter dropdown.</summary>
    [HttpGet("queue-titles")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetQueueTitles(CancellationToken cancellationToken)
    {
        var titles = await _reportService.GetQueueTitlesAsync(cancellationToken);
        return Ok(titles);
    }
}
