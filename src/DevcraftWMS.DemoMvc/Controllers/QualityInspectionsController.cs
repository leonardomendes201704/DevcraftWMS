using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.QualityInspections;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class QualityInspectionsController : Controller
{
    private readonly QualityInspectionsApiClient _client;

    public QualityInspectionsController(QualityInspectionsApiClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] QualityInspectionQuery query, CancellationToken cancellationToken)
    {
        var result = await _client.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load quality inspections.";
            return View(new QualityInspectionListPageViewModel { Query = query });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "QualityInspections",
            Query = new Dictionary<string, string?>
            {
                ["Status"] = query.Status?.ToString(),
                ["WarehouseId"] = query.WarehouseId?.ToString(),
                ["ReceiptId"] = query.ReceiptId?.ToString(),
                ["ProductId"] = query.ProductId?.ToString(),
                ["LotId"] = query.LotId?.ToString(),
                ["CreatedFromUtc"] = query.CreatedFromUtc?.ToString("O"),
                ["CreatedToUtc"] = query.CreatedToUtc?.ToString("O"),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        return View(new QualityInspectionListPageViewModel
        {
            Items = result.Data.Items,
            Pagination = pagination,
            Query = query
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Quality inspection not found.";
            return RedirectToAction(nameof(Index));
        }

        var evidenceResult = await _client.ListEvidenceAsync(id, cancellationToken);
        if (!evidenceResult.IsSuccess)
        {
            TempData["Error"] = evidenceResult.Error ?? "Failed to load evidence.";
        }

        var model = new QualityInspectionDetailPageViewModel
        {
            Inspection = result.Data,
            Evidence = evidenceResult.Data ?? Array.Empty<QualityInspectionEvidenceViewModel>(),
            Decision = new QualityInspectionDecisionFormViewModel
            {
                InspectionId = id
            }
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadEvidence(Guid inspectionId, Guid evidenceId, CancellationToken cancellationToken)
    {
        var result = await _client.DownloadEvidenceAsync(inspectionId, evidenceId, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to download evidence.";
            return RedirectToAction(nameof(Details), new { id = inspectionId });
        }

        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpPost]
    public async Task<IActionResult> Approve([Bind(Prefix = "Decision")] QualityInspectionDecisionFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Fill in the required fields.";
            return RedirectToAction(nameof(Details), new { id = model.InspectionId });
        }

        var result = await _client.ApproveAsync(model.InspectionId, model.Notes, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to approve inspection.";
            return RedirectToAction(nameof(Details), new { id = model.InspectionId });
        }

        if (model.EvidenceFile is not null && model.EvidenceFile.Length > 0)
        {
            var upload = await _client.AddEvidenceAsync(model.InspectionId, model.EvidenceFile, cancellationToken);
            if (!upload.IsSuccess)
            {
                TempData["Warning"] = upload.Error ?? "Inspection approved, but evidence upload failed.";
            }
        }

        TempData["Success"] = "Inspection approved and lot released.";
        return RedirectToAction(nameof(Details), new { id = model.InspectionId });
    }

    [HttpPost]
    public async Task<IActionResult> Reject([Bind(Prefix = "Decision")] QualityInspectionDecisionFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Fill in the required fields.";
            return RedirectToAction(nameof(Details), new { id = model.InspectionId });
        }

        var result = await _client.RejectAsync(model.InspectionId, model.Notes, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to reject inspection.";
            return RedirectToAction(nameof(Details), new { id = model.InspectionId });
        }

        if (model.EvidenceFile is not null && model.EvidenceFile.Length > 0)
        {
            var upload = await _client.AddEvidenceAsync(model.InspectionId, model.EvidenceFile, cancellationToken);
            if (!upload.IsSuccess)
            {
                TempData["Warning"] = upload.Error ?? "Inspection rejected, but evidence upload failed.";
            }
        }

        TempData["Success"] = "Inspection rejected and quarantine maintained.";
        return RedirectToAction(nameof(Details), new { id = model.InspectionId });
    }
}
