using Microsoft.Extensions.Options;
using SiloSync.Service.Options;

namespace SiloSync.Service.Serivces;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly SiloSyncOptions _options;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IOptions<SiloSyncOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_options.PollIntervalMinutes));

        _logger.LogInformation("SiloSync worker started; polling every {PollIntervalMinutes} minute(s)", _options.PollIntervalMinutes);

        do
        {
            try
            {
                await ProcessCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker cancellation requested; stopping.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error during sync cycle.");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ProcessCycleAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting sync cycle.");

        using var scope = _serviceProvider.CreateScope();
        var reader = scope.ServiceProvider.GetRequiredService<IGalleryReader>();
        var stateStore = scope.ServiceProvider.GetRequiredService<ISyncStateStore>();
        var uploader = scope.ServiceProvider.GetRequiredService<IFileUploader>();

        var lastId = await stateStore.GetLastGalleryIdAsync(cancellationToken);
        var rows = await reader.GetNewFilesAsync(lastId, _options.BatchSize, cancellationToken);

        if (rows.Count == 0)
        {
            _logger.LogDebug("No new gallery rows found after id {LastId}.", lastId);
            return;
        }

        _logger.LogInformation("Found {Count} new gallery row(s) after id {LastId}.", rows.Count, lastId);

        long maxProcessedId = lastId;
        var mediaRootPath = _options.SqlServer.MediaRootPath;

        foreach (var row in rows)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row.FldGalleryUsageId))
                {
                    _logger.LogWarning("Skipping gallery row {GalleryId}: missing usage id.", row.FldGalleryId);
                    maxProcessedId = Math.Max(maxProcessedId, row.FldGalleryId);
                    continue;
                }

                if (await stateStore.IsSentAsync(row.FldGalleryId, cancellationToken))
                {
                    _logger.LogDebug("Skipping already sent gallery row {GalleryId}.", row.FldGalleryId);
                    maxProcessedId = Math.Max(maxProcessedId, row.FldGalleryId);
                    continue;
                }

                var fullPath = ResolveFullPath(mediaRootPath, row.FldGalleryMediaPath);
                if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
                {
                    _logger.LogWarning("Skipping gallery row {GalleryId}: file not found at {FilePath}.", row.FldGalleryId, fullPath);
                    continue;
                }

                await using var fileStream = File.OpenRead(fullPath);
                var response = await uploader.UploadAsync(row, fileStream, cancellationToken);

                if (response.Succeeded)
                {
                    await stateStore.MarkSentAsync(
                        row.FldGalleryId,
                        row.FldGalleryUsageId,
                        row.FldGalleryMediaName,
                        fullPath,
                        response.Url,
                        cancellationToken);

                    _logger.LogInformation("Uploaded gallery row {GalleryId} file {FileName} to {Url}", row.FldGalleryId, response.FileName, response.Url);
                }
                else
                {
                    _logger.LogWarning("Upload failed for gallery row {GalleryId}: {Error}", row.FldGalleryId, response.Error);
                    continue;
                }

                maxProcessedId = Math.Max(maxProcessedId, row.FldGalleryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing gallery row {GalleryId}.", row.FldGalleryId);
            }
        }

        await stateStore.UpdateWatermarkAsync(maxProcessedId, cancellationToken);
        _logger.LogDebug("Sync cycle complete; watermark advanced to {MaxProcessedId}.", maxProcessedId);
    }

    private static string? ResolveFullPath(string mediaRootPath, string? relativeOrFullPath)
    {
        if (string.IsNullOrWhiteSpace(relativeOrFullPath))
        {
            return null;
        }

        if (Path.IsPathRooted(relativeOrFullPath))
        {
            return relativeOrFullPath;
        }

        return Path.Combine(mediaRootPath, relativeOrFullPath.TrimStart('\\', '/'));
    }
}
