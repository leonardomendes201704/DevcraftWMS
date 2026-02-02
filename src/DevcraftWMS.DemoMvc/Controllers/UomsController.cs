using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Uoms;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class UomsController : Controller
{
    private readonly UomsApiClient _uomsClient;

    public UomsController(UomsApiClient uomsClient)
    {
        _uomsClient = uomsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] UomQuery query, CancellationToken cancellationToken)
    {
        var result = await _uomsClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load units of measure.";
            return View(new UomListPageViewModel { Query = query });
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Uoms",
            Query = new Dictionary<string, string?>
            {
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["Code"] = query.Code,
                ["Name"] = query.Name,
                ["Type"] = query.Type?.ToString(),
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        var model = new UomListPageViewModel
        {
            Items = result.Data.Items,
            Query = query,
            Pagination = pagination
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _uomsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "UoM not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new UomFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(UomFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _uomsClient.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create UoM.");
            return View(model);
        }

        TempData["Success"] = "UoM created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _uomsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "UoM not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new UomFormViewModel
        {
            Id = result.Data.Id,
            Code = result.Data.Code,
            Name = result.Data.Name,
            Type = result.Data.Type
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UomFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            return View(model);
        }

        var result = await _uomsClient.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update UoM.");
            return View(model);
        }

        TempData["Success"] = "UoM updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _uomsClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "UoM not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _uomsClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate UoM.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "UoM deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }
}
