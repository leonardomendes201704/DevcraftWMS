using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.CostCenters;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class CostCentersController : Controller
{
    private readonly CostCentersApiClient _client;

    public CostCentersController(CostCentersApiClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] CostCenterQuery query, CancellationToken cancellationToken)
    {
        var result = await _client.ListAsync(query, cancellationToken);
        var model = new CostCenterListPageViewModel
        {
            Items = result.Data?.Items ?? Array.Empty<CostCenterListItemViewModel>(),
            Pagination = result.Data is null
                ? new PaginationViewModel()
                : new PaginationViewModel
                {
                    PageNumber = result.Data.PageNumber,
                    PageSize = result.Data.PageSize,
                    TotalCount = result.Data.TotalCount,
                    Action = nameof(Index),
                    Controller = "CostCenters",
                    Query = new Dictionary<string, string?>
                    {
                        ["OrderBy"] = query.OrderBy,
                        ["OrderDir"] = query.OrderDir,
                        ["Code"] = query.Code,
                        ["Name"] = query.Name,
                        ["IsActive"] = query.IsActive?.ToString(),
                        ["IncludeInactive"] = query.IncludeInactive.ToString(),
                        ["PageSize"] = query.PageSize.ToString()
                    }
                },
            Query = query
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CostCenterFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CostCenterFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _client.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create cost center.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Cost center created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data?.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = new CostCenterFormViewModel
        {
            Id = result.Data.Id,
            Code = result.Data.Code,
            Name = result.Data.Name,
            Description = result.Data.Description
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CostCenterFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _client.UpdateAsync(id, model, cancellationToken);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to update cost center.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Cost center updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _client.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error ?? "Unable to deactivate cost center.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["SuccessMessage"] = "Cost center deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }
}
