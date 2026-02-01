namespace PerfectApiTemplate.Application.Common.Pagination;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir);

