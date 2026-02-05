using DevcraftWMS.DemoMvc.ViewModels.Roles;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class RolesApiClient : ApiClientBase
{
    public RolesApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider,
        Infrastructure.Telemetry.IClientCorrelationContext correlationContext,
        Infrastructure.Telemetry.IClientTelemetryDispatcher telemetryDispatcher,
        Microsoft.Extensions.Options.IOptionsMonitor<Infrastructure.Telemetry.ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
        : base(httpClient, httpContextAccessor, urlProvider, correlationContext, telemetryDispatcher, telemetryOptions, environment)
    {
    }

    public Task<ApiResult<PagedResultDto<RoleListItemViewModel>>> ListAsync(RoleQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/roles";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["search"] = query.Search,
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<RoleListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<RoleDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<RoleDetailViewModel>($"/api/roles/{id}", cancellationToken);

    public Task<ApiResult<RoleDetailViewModel>> CreateAsync(RoleFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<RoleDetailViewModel>("/api/roles", new CreateRoleRequest(payload.Name, payload.Description, payload.PermissionIds), cancellationToken);

    public Task<ApiResult<RoleDetailViewModel>> UpdateAsync(Guid id, RoleFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<RoleDetailViewModel>($"/api/roles/{id}", new UpdateRoleRequest(payload.Name, payload.Description, payload.PermissionIds), cancellationToken);

    public Task<ApiResult<RoleDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<RoleDetailViewModel>($"/api/roles/{id}", cancellationToken);
}

public sealed record CreateRoleRequest(string Name, string? Description, IReadOnlyList<Guid> PermissionIds);

public sealed record UpdateRoleRequest(string Name, string? Description, IReadOnlyList<Guid> PermissionIds);
