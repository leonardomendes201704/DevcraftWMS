namespace PerfectApiTemplate.Application.Common.Pagination;

public sealed record CursorPaginationRequest(
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Cursor = null,
    bool IncludeInactive = false);

public sealed record CursorPaginationResult<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    int PageSize,
    string OrderBy,
    string OrderDir);
