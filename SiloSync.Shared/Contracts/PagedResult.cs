namespace SiloSync.Shared.Contracts;

/// <summary>
/// A single page of results plus enough metadata to render pagination controls.
/// </summary>
public sealed record PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }

    public required int TotalCount { get; init; }

    public required int PageNumber { get; init; }

    public required int PageSize { get; init; }
}
