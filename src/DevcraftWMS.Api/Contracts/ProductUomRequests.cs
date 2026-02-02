namespace DevcraftWMS.Api.Contracts;

public sealed record AddProductUomRequest(Guid UomId, decimal ConversionFactor);
