namespace DevcraftWMS.Portal.ApiClients;

public sealed record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir);
