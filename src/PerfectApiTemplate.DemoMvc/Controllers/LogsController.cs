using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.DemoMvc.ApiClients;
using PerfectApiTemplate.DemoMvc.ViewModels.Logs;
using PerfectApiTemplate.DemoMvc.ViewModels.Shared;

namespace PerfectApiTemplate.DemoMvc.Controllers;

public sealed class LogsController : Controller
{
    private readonly LogsApiClient _client;

    public LogsController(LogsApiClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<IActionResult> Requests([FromQuery] RequestLogFilters filters, CancellationToken cancellationToken)
    {
        var query = new RequestLogQuery(filters.PageNumber, filters.PageSize, filters.OrderBy, filters.OrderDir, filters.StatusCode, filters.PathContains, filters.FromUtc, filters.ToUtc);
        var result = await _client.ListRequestsAsync(query, cancellationToken);

        var model = new RequestLogsViewModel
        {
            Items = result.Data?.Items ?? Array.Empty<RequestLogDto>(),
            Filters = filters,
            Pagination = BuildPagination("Requests", filters.PageNumber, filters.PageSize, result.Data?.TotalCount ?? 0, new Dictionary<string, string?>
            {
                ["OrderBy"] = filters.OrderBy,
                ["OrderDir"] = filters.OrderDir,
                ["StatusCode"] = filters.StatusCode?.ToString(),
                ["PathContains"] = filters.PathContains,
                ["FromUtc"] = filters.FromUtc?.ToString("O"),
                ["ToUtc"] = filters.ToUtc?.ToString("O"),
                ["PageSize"] = filters.PageSize.ToString()
            })
        };

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to load request logs.";
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Errors([FromQuery] ErrorLogFilters filters, CancellationToken cancellationToken)
    {
        var query = new ErrorLogQuery(filters.PageNumber, filters.PageSize, filters.OrderBy, filters.OrderDir, null, null, null, filters.ExceptionType, filters.FromUtc, filters.ToUtc);
        var result = await _client.ListErrorsAsync(query, cancellationToken);

        var model = new ErrorLogsViewModel
        {
            Items = result.Data?.Items ?? Array.Empty<ErrorLogDto>(),
            Filters = filters,
            Pagination = BuildPagination("Errors", filters.PageNumber, filters.PageSize, result.Data?.TotalCount ?? 0, new Dictionary<string, string?>
            {
                ["OrderBy"] = filters.OrderBy,
                ["OrderDir"] = filters.OrderDir,
                ["ExceptionType"] = filters.ExceptionType,
                ["FromUtc"] = filters.FromUtc?.ToString("O"),
                ["ToUtc"] = filters.ToUtc?.ToString("O"),
                ["PageSize"] = filters.PageSize.ToString()
            })
        };

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to load error logs.";
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Error(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetErrorAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Error log not found.";
            return RedirectToAction(nameof(Errors));
        }

        return View(new ErrorLogDetailViewModel { Log = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> Transactions([FromQuery] TransactionLogFilters filters, CancellationToken cancellationToken)
    {
        var query = new TransactionLogQuery(filters.PageNumber, filters.PageSize, filters.OrderBy, filters.OrderDir, filters.EntityName, filters.Operation, filters.FromUtc, filters.ToUtc);
        var result = await _client.ListTransactionsAsync(query, cancellationToken);

        var model = new TransactionLogsViewModel
        {
            Items = result.Data?.Items ?? Array.Empty<TransactionLogDto>(),
            Filters = filters,
            Pagination = BuildPagination("Transactions", filters.PageNumber, filters.PageSize, result.Data?.TotalCount ?? 0, new Dictionary<string, string?>
            {
                ["OrderBy"] = filters.OrderBy,
                ["OrderDir"] = filters.OrderDir,
                ["EntityName"] = filters.EntityName,
                ["Operation"] = filters.Operation,
                ["FromUtc"] = filters.FromUtc?.ToString("O"),
                ["ToUtc"] = filters.ToUtc?.ToString("O"),
                ["PageSize"] = filters.PageSize.ToString()
            })
        };

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to load transaction logs.";
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Transaction(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetTransactionAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Transaction log not found.";
            return RedirectToAction(nameof(Transactions));
        }

        return View(new TransactionLogDetailViewModel { Log = result.Data });
    }

    private static PaginationViewModel BuildPagination(string action, int pageNumber, int pageSize, int total, Dictionary<string, string?> query)
    {
        return new PaginationViewModel
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total,
            Action = action,
            Controller = "Logs",
            Query = query
        };
    }
}
