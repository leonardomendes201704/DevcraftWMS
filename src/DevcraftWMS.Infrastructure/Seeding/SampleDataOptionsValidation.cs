using Microsoft.Extensions.Options;

namespace DevcraftWMS.Infrastructure.Seeding;

public sealed class SampleDataOptionsValidation : IValidateOptions<SampleDataOptions>
{
    public ValidateOptionsResult Validate(string? name, SampleDataOptions options)
    {
        if (!options.Enabled)
        {
            return ValidateOptionsResult.Success;
        }

        if (options.CustomerId == Guid.Empty)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:CustomerId is required when sample data seeding is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.CustomerName))
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:CustomerName is required when sample data seeding is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.CustomerEmail))
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:CustomerEmail is required when sample data seeding is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.WarehouseCode))
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:WarehouseCode is required when sample data seeding is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.WarehouseName))
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:WarehouseName is required when sample data seeding is enabled.");
        }

        if (options.ProductCount <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:ProductCount must be greater than zero.");
        }

        if (options.LotsPerProduct <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:LotsPerProduct must be greater than zero.");
        }

        if (options.LotExpirationWindowDays <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:LotExpirationWindowDays must be greater than zero.");
        }

        if (options.MovementCount < 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:MovementCount must be zero or greater.");
        }

        if (options.MovementPerformedWindowDays <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:MovementPerformedWindowDays must be greater than zero.");
        }

        if (options.MovementQuantityMin <= 0 || options.MovementQuantityMax <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:MovementQuantityMin/Max must be greater than zero.");
        }

        if (options.MovementQuantityMax < options.MovementQuantityMin)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:MovementQuantityMax must be greater than or equal to MovementQuantityMin.");
        }

        if (options.PickingTaskCount < 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:PickingTaskCount must be zero or greater.");
        }

        if (options.InboundOrderCount <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:InboundOrderCount must be greater than zero.");
        }

        if (options.ReceiptItemsPerOrder <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:ReceiptItemsPerOrder must be greater than zero.");
        }

        if (options.UnitLoadsPerOrder <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:UnitLoadsPerOrder must be greater than zero.");
        }

        if (options.InventoryCountCount <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:InventoryCountCount must be greater than zero.");
        }

        if (options.InventoryCountItemsPerCount <= 0)
        {
            return ValidateOptionsResult.Fail("Seed:SampleData:InventoryCountItemsPerCount must be greater than zero.");
        }

        return ValidateOptionsResult.Success;
    }
}
