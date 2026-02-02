using DevcraftWMS.DemoMvc.ViewModels.Structures;
using DevcraftWMS.DemoMvc.Enums;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class StructuresApiClient : ApiClientBase
{
    public StructuresApiClient(
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

    public Task<ApiResult<PagedResultDto<StructureListItemViewModel>>> ListAsync(StructureQuery query, CancellationToken cancellationToken)
    {
        var basePath = $"/api/sections/{query.SectionId}/structures";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["code"] = query.Code,
            ["name"] = query.Name,
            ["structureType"] = query.StructureType?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<StructureListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<PagedResultDto<StructureListItemViewModel>>> ListForCustomerAsync(StructureQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/structures";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["code"] = query.Code,
            ["name"] = query.Name,
            ["structureType"] = query.StructureType?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<StructureListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<StructureDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<StructureDetailViewModel>($"/api/structures/{id}", cancellationToken);

    public Task<ApiResult<StructureDetailViewModel>> CreateAsync(Guid sectionId, StructureFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<StructureDetailViewModel>($"/api/sections/{sectionId}/structures", new StructureCreateRequest(
            sectionId,
            payload.Code,
            payload.Name,
            payload.StructureType,
            payload.Levels), cancellationToken);

    public Task<ApiResult<StructureDetailViewModel>> UpdateAsync(Guid id, StructureFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<StructureDetailViewModel>($"/api/structures/{id}", new StructureUpdateRequest(
            payload.SectionId,
            payload.Code,
            payload.Name,
            payload.StructureType,
            payload.Levels), cancellationToken);

    public Task<ApiResult<StructureDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<StructureDetailViewModel>($"/api/structures/{id}", cancellationToken);
}

public sealed record StructureCreateRequest(Guid SectionId, string Code, string Name, StructureType StructureType, int Levels);

public sealed record StructureUpdateRequest(Guid SectionId, string Code, string Name, StructureType StructureType, int Levels);
