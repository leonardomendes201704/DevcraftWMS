namespace DevcraftWMS.Application.Features.Receipts;

public sealed class ReceiptMeasurementOptions
{
    public const string SectionName = "ReceiptMeasurements";

    public bool BlockOnDeviation { get; init; }
    public decimal MaxWeightDeviationPercent { get; init; } = 10m;
    public decimal MaxVolumeDeviationPercent { get; init; } = 10m;
}
