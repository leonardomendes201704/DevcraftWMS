namespace DevcraftWMS.Api.Contracts;

public sealed record CreateLocationRequest(
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    Guid? ZoneId,
    decimal? MaxWeightKg,
    decimal? MaxVolumeM3,
    bool AllowLotTracking,
    bool AllowExpiryTracking);

public sealed record UpdateLocationRequest(
    Guid StructureId,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    Guid? ZoneId,
    decimal? MaxWeightKg,
    decimal? MaxVolumeM3,
    bool AllowLotTracking,
    bool AllowExpiryTracking);
