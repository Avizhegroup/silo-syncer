using Microsoft.AspNetCore.Mvc;
using SiloSync.Api.Services;
using SiloSync.Shared.Contracts;

namespace SiloSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileStorageService fileStorage, ILogger<FilesController> logger)
    {
        _fileStorage = fileStorage;
        _logger = logger;
    }

    [HttpPost]
    [RequestSizeLimit(500L * 1024 * 1024)]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = 500L * 1024 * 1024)]
    public async Task<IActionResult> Upload(CancellationToken cancellationToken)
    {
        var form = await Request.ReadFormAsync(cancellationToken);

        var usageId = form["usageId"].FirstOrDefault();
        var file = form.Files.GetFile("file");

        if (string.IsNullOrWhiteSpace(usageId))
        {
            return BadRequest(new { Error = "usageId is required." });
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { Error = "file is required." });
        }

        var fileName = file.FileName;
        await using var stream = file.OpenReadStream();
        var result = await _fileStorage.SaveFileAsync(usageId, fileName, stream, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Upload rejected for usage {UsageId}: {Error}", usageId, result.Error);
            return BadRequest(new { result.Error });
        }

        return Ok(result);
    }

    [HttpGet("{usageId}")]
    public ActionResult<IReadOnlyList<FileLinkDto>> GetByUsageId(string usageId)
    {
        var links = _fileStorage.GetFileLinksByUsageId(usageId);
        return Ok(links);
    }

    [HttpGet("{usageId}/{fileName}")]
    public async Task<IActionResult> Download(string usageId, string fileName, CancellationToken cancellationToken)
    {
        var stream = await _fileStorage.GetFileStreamAsync(usageId, fileName, cancellationToken);

        if (stream is null)
        {
            return NotFound();
        }

        var contentType = _fileStorage.GetContentType(fileName) ?? "application/octet-stream";
        return File(stream, contentType, fileName);
    }
}
