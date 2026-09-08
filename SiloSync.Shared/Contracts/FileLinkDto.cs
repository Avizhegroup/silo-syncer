namespace SiloSync.Shared.Contracts;

public sealed record FileLinkDto
{
    public required string UsageId { get; init; }

    public required string FileName { get; init; }

    public required string Url { get; init; }
}
