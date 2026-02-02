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

        return ValidateOptionsResult.Success;
    }
}
