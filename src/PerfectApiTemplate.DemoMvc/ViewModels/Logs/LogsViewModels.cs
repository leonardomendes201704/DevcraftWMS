using PerfectApiTemplate.DemoMvc.ApiClients;
using PerfectApiTemplate.DemoMvc.ViewModels.Shared;

namespace PerfectApiTemplate.DemoMvc.ViewModels.Logs;

public sealed class RequestLogsViewModel
{
    public IReadOnlyList<RequestLogDto> Items { get; init; } = Array.Empty<RequestLogDto>();
    public RequestLogFilters Filters { get; init; } = new();
    public PaginationViewModel Pagination { get; init; } = new();
}

public sealed class ErrorLogsViewModel
{
    public IReadOnlyList<ErrorLogDto> Items { get; init; } = Array.Empty<ErrorLogDto>();
    public ErrorLogFilters Filters { get; init; } = new();
    public PaginationViewModel Pagination { get; init; } = new();
}

public sealed class ClientErrorLogsViewModel
{
    public IReadOnlyList<ErrorLogDto> Items { get; init; } = Array.Empty<ErrorLogDto>();
    public ClientErrorLogFilters Filters { get; init; } = new();
    public PaginationViewModel Pagination { get; init; } = new();
}

public sealed class TransactionLogsViewModel
{
    public IReadOnlyList<TransactionLogDto> Items { get; init; } = Array.Empty<TransactionLogDto>();
    public TransactionLogFilters Filters { get; init; } = new();
    public PaginationViewModel Pagination { get; init; } = new();
}

public sealed class ErrorLogDetailViewModel
{
    public ErrorLogDetailDto? Log { get; init; }
}

public sealed class TransactionLogDetailViewModel
{
    public TransactionLogDetailDto? Log { get; init; }
}

public sealed class RequestLogFilters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string OrderBy { get; set; } = "StartedAtUtc";
    public string OrderDir { get; set; } = "desc";
    public int? StatusCode { get; set; }
    public string? PathContains { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}

public sealed class ErrorLogFilters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string OrderBy { get; set; } = "CreatedAtUtc";
    public string OrderDir { get; set; } = "desc";
    public string? ExceptionType { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}

public sealed class ClientErrorLogFilters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string OrderBy { get; set; } = "CreatedAtUtc";
    public string OrderDir { get; set; } = "desc";
    public string? EventType { get; set; }
    public string? Severity { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}

public sealed class TransactionLogFilters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string OrderBy { get; set; } = "CreatedAtUtc";
    public string OrderDir { get; set; } = "desc";
    public string? EntityName { get; set; }
    public string? Operation { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}
