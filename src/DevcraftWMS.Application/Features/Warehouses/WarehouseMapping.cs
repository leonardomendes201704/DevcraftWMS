using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Warehouses;

public static class WarehouseMapping
{
    public static WarehouseDto MapWarehouse(Warehouse warehouse)
    {
        var addresses = warehouse.Addresses
            .Select(a => new WarehouseAddressDto(a.Id, a.IsPrimary, a.AddressLine1, a.AddressLine2, a.District, a.City, a.State, a.PostalCode, a.Country, a.Latitude, a.Longitude))
            .ToList();
        var contacts = warehouse.Contacts
            .Select(c => new WarehouseContactDto(c.Id, c.IsPrimary, c.ContactName, c.ContactEmail, c.ContactPhone))
            .ToList();
        var capacities = warehouse.Capacities
            .Select(c => new WarehouseCapacityDto(c.Id, c.IsPrimary, c.LengthMeters, c.WidthMeters, c.HeightMeters, c.TotalAreaM2, c.TotalCapacity, c.CapacityUnit, c.MaxWeightKg, c.OperationalArea))
            .ToList();

        return new WarehouseDto(
            warehouse.Id,
            warehouse.Code,
            warehouse.Name,
            warehouse.ShortName,
            warehouse.Description,
            warehouse.WarehouseType,
            warehouse.IsPrimary,
            warehouse.IsPickingEnabled,
            warehouse.IsReceivingEnabled,
            warehouse.IsShippingEnabled,
            warehouse.IsReturnsEnabled,
            warehouse.ExternalId,
            warehouse.ErpCode,
            warehouse.CostCenterCode,
            warehouse.CostCenterName,
            warehouse.CutoffTime,
            warehouse.Timezone,
            warehouse.IsActive,
            warehouse.CreatedAtUtc,
            addresses,
            contacts,
            capacities);
    }

    public static WarehouseListItemDto MapListItem(Warehouse warehouse)
    {
        var primaryAddress = warehouse.Addresses.FirstOrDefault(a => a.IsPrimary) ?? warehouse.Addresses.FirstOrDefault();
        return new WarehouseListItemDto(
            warehouse.Id,
            warehouse.Code,
            warehouse.Name,
            warehouse.WarehouseType,
            warehouse.IsPrimary,
            warehouse.IsActive,
            primaryAddress?.City,
            primaryAddress?.State,
            primaryAddress?.Country,
            warehouse.CreatedAtUtc);
    }
}
