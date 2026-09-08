namespace SiloSync.Shared.Contracts;

public sealed record UploadFileResponse
{
    public bool Succeeded { get; init; }

    public string? UsageId { get; init; }

    public string? FileName { get; init; }

    public string? Url { get; init; }

    public string? Error { get; init; }
}
