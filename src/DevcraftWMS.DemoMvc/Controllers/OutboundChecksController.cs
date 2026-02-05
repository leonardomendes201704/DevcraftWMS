using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.OutboundChecks;
using DevcraftWMS.DemoMvc.ViewModels.OutboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class OutboundChecksController : Controller
{
    private readonly OutboundOrdersApiClient _ordersClient;
    private readonly OutboundChecksApiClient _checksClient;

    public OutboundChecksController(OutboundOrdersApiClient ordersClient, OutboundChecksApiClient checksClient)
    {
        _ordersClient = ordersClient;
        _checksClient = checksClient;
    }

    public async Task<IActionResult> Index([FromQuery] OutboundOrderQuery query, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Unable to load outbound orders.";
            return View(new OutboundCheckQueuePageViewModel
            {
                Items = Array.Empty<OutboundOrderListItemViewModel>(),
                Query = query
            });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "OutboundChecks",
            Query = new Dictionary<string, string?>
            {
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["OrderNumber"] = query.OrderNumber,
                ["Status"] = query.Status?.ToString(),
                ["Priority"] = query.Priority?.ToString(),
                ["CreatedFromUtc"] = query.CreatedFromUtc?.ToString("O"),
                ["CreatedToUtc"] = query.CreatedToUtc?.ToString("O"),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new OutboundCheckQueuePageViewModel
        {
            Items = result.Data.Items,
            Query = query,
            Pagination = pagination
        });
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _ordersClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Outbound order not found.";
            return RedirectToAction(nameof(Index));
        }

        var checkItems = result.Data.Items.Select(item => new OutboundCheckItemInputViewModel
        {
            OutboundOrderItemId = item.Id,
            ProductCode = item.ProductCode,
            ProductName = item.ProductName,
            UomCode = item.UomCode,
            QuantityExpected = item.Quantity,
            QuantityChecked = item.Quantity
        }).ToList();

        return View(new OutboundCheckDetailsPageViewModel
        {
            Order = result.Data,
            Check = new OutboundCheckSubmitViewModel
            {
                Items = checkItems
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Check(Guid id, OutboundCheckSubmitViewModel model, CancellationToken cancellationToken)
    {
        if (model.Items.Count == 0)
        {
            TempData["Error"] = "Please provide at least one item to check.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var items = new List<OutboundCheckItemRequestViewModel>();
        foreach (var item in model.Items)
        {
            var evidence = new List<OutboundCheckEvidenceRequestViewModel>();
            if (item.EvidenceFile is not null && item.EvidenceFile.Length > 0)
            {
                await using var stream = new MemoryStream();
                await item.EvidenceFile.CopyToAsync(stream, cancellationToken);
                evidence.Add(new OutboundCheckEvidenceRequestViewModel
                {
                    FileName = item.EvidenceFile.FileName,
                    ContentType = item.EvidenceFile.ContentType,
                    SizeBytes = item.EvidenceFile.Length,
                    Content = stream.ToArray()
                });
            }

            items.Add(new OutboundCheckItemRequestViewModel
            {
                OutboundOrderItemId = item.OutboundOrderItemId,
                QuantityChecked = item.QuantityChecked,
                DivergenceReason = item.DivergenceReason,
                Evidence = evidence.Count == 0 ? null : evidence
            });
        }

        var result = await _checksClient.RegisterAsync(id, new OutboundCheckRequestViewModel
        {
            Items = items,
            Notes = model.Notes
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Unable to register outbound check.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Outbound check registered successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
