using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.DemoMvc.ApiClients;
using PerfectApiTemplate.DemoMvc.ViewModels.Logs;
using PerfectApiTemplate.DemoMvc.ViewModels.Shared;

namespace PerfectApiTemplate.DemoMvc.Controllers;

public sealed class ClientLogsController : Controller
{
    private const string ClientSource = "client";
    private readonly LogsApiClient _client;

    public ClientLogsController(LogsApiClient client)
    {
        _client = client;
    }

    public async Task<IActionResult> Errors([FromQuery] ClientErrorLogFilters filters, CancellationToken cancellationToken)
    {
        var query = new ErrorLogQuery(
            filters.PageNumber,
            filters.PageSize,
            filters.OrderBy,
            filters.OrderDir,
            ClientSource,
            filters.EventType,
            filters.Severity,
            null,
            filters.FromUtc,
            filters.ToUtc);

        var result = await _client.ListErrorsAsync(query, cancellationToken);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to load client error logs.";
        }

        var model = new ClientErrorLogsViewModel
        {
            Items = result.Data?.Items ?? Array.Empty<ErrorLogDto>(),
            Filters = filters,
            Pagination = BuildPagination(filters, result.Data?.TotalCount ?? 0)
        };

        return View(model);
    }

    public async Task<IActionResult> Error(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetErrorAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Client error log not found.";
        }

        return View(new ErrorLogDetailViewModel { Log = result.Data });
    }

    private static PaginationViewModel BuildPagination(ClientErrorLogFilters filters, int total)
    {
        return new PaginationViewModel
        {
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize,
            TotalCount = total,
            Action = "Errors",
            Controller = "ClientLogs",
            Query = new Dictionary<string, string?>
            {
                ["OrderBy"] = filters.OrderBy,
                ["OrderDir"] = filters.OrderDir,
                ["EventType"] = filters.EventType,
                ["Severity"] = filters.Severity,
                ["FromUtc"] = filters.FromUtc?.ToString("O"),
                ["ToUtc"] = filters.ToUtc?.ToString("O"),
                ["PageSize"] = filters.PageSize.ToString()
            }
        };
    }
}
