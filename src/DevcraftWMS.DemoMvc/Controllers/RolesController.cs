using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ApiClients;
using DevcraftWMS.DemoMvc.ViewModels.Roles;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.Controllers;

public sealed class RolesController : Controller
{
    private readonly RolesApiClient _rolesClient;
    private readonly PermissionsApiClient _permissionsClient;

    public RolesController(RolesApiClient rolesClient, PermissionsApiClient permissionsClient)
    {
        _rolesClient = rolesClient;
        _permissionsClient = permissionsClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] RoleQuery query, CancellationToken cancellationToken)
    {
        var result = await _rolesClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load roles.";
            return View(new RoleListPageViewModel());
        }

        var pagination = new PaginationViewModel
        {
            PageNumber = result.Data.PageNumber,
            PageSize = result.Data.PageSize,
            TotalCount = result.Data.TotalCount,
            Action = nameof(Index),
            Controller = "Roles",
            Query = new Dictionary<string, string?>
            {
                ["OrderBy"] = query.OrderBy,
                ["OrderDir"] = query.OrderDir,
                ["Search"] = query.Search,
                ["IsActive"] = query.IsActive?.ToString(),
                ["IncludeInactive"] = query.IncludeInactive.ToString(),
                ["PageSize"] = query.PageSize.ToString()
            }
        };

        var model = new RoleListPageViewModel
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
        var result = await _rolesClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Role not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new RoleDetailsPageViewModel { Role = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new RoleFormViewModel
        {
            AvailablePermissions = await LoadPermissionsAsync(Array.Empty<Guid>(), cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoleFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.AvailablePermissions = await LoadPermissionsAsync(model.PermissionIds, cancellationToken);
            return View(model);
        }

        var result = await _rolesClient.CreateAsync(model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create role.");
            model.AvailablePermissions = await LoadPermissionsAsync(model.PermissionIds, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Role created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _rolesClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Role not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new RoleFormViewModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name,
            Description = result.Data.Description,
            PermissionIds = result.Data.Permissions.Select(p => p.Id).ToList(),
            AvailablePermissions = await LoadPermissionsAsync(result.Data.Permissions.Select(p => p.Id).ToList(), cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(RoleFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.Id is null)
        {
            model.AvailablePermissions = await LoadPermissionsAsync(model.PermissionIds, cancellationToken);
            return View(model);
        }

        var result = await _rolesClient.UpdateAsync(model.Id.Value, model, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update role.");
            model.AvailablePermissions = await LoadPermissionsAsync(model.PermissionIds, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Role updated successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _rolesClient.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Role not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new RoleDetailsPageViewModel { Role = result.Data });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _rolesClient.DeactivateAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error ?? "Failed to deactivate role.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Role deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<SelectListItem>> LoadPermissionsAsync(IReadOnlyCollection<Guid> selected, CancellationToken cancellationToken)
    {
        var query = new PermissionQuery(1, 100, "Code", "asc", null, true, true);
        var result = await _permissionsClient.ListAsync(query, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "Failed to load permissions.";
            return Array.Empty<SelectListItem>();
        }

        return result.Data.Items
            .OrderBy(p => p.Code)
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Code} - {p.Name}",
                Selected = selected.Contains(p.Id)
            })
            .ToList();
    }
}
