using Microsoft.Extensions.Options;

namespace DevcraftWMS.Application.Features.Receipts;

public sealed class ReceiptMeasurementOptionsValidation : IValidateOptions<ReceiptMeasurementOptions>
{
    public ValidateOptionsResult Validate(string? name, ReceiptMeasurementOptions options)
    {
        if (options.MaxWeightDeviationPercent <= 0)
        {
            return ValidateOptionsResult.Fail("ReceiptMeasurements:MaxWeightDeviationPercent must be greater than zero.");
        }

        if (options.MaxVolumeDeviationPercent <= 0)
        {
            return ValidateOptionsResult.Fail("ReceiptMeasurements:MaxVolumeDeviationPercent must be greater than zero.");
        }

        return ValidateOptionsResult.Success;
    }
}
