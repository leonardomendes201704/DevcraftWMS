using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Warehouses.Commands.UpdateWarehouse;

public sealed record UpdateWarehouseCommand(
    Guid Id,
    string Code,
    string Name,
    string? ShortName,
    string? Description,
    WarehouseType WarehouseType,
    bool IsPrimary,
    bool IsPickingEnabled,
    bool IsReceivingEnabled,
    bool IsShippingEnabled,
    bool IsReturnsEnabled,
    string? ExternalId,
    string? ErpCode,
    string? CostCenterCode,
    string? CostCenterName,
    TimeOnly? CutoffTime,
    string? Timezone,
    WarehouseAddressInput? Address,
    WarehouseContactInput? Contact,
    WarehouseCapacityInput? Capacity) : IRequest<RequestResult<WarehouseDto>>;
