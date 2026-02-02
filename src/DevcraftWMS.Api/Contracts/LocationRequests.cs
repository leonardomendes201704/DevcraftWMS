namespace DevcraftWMS.Api.Contracts;

public sealed record CreateLocationRequest(string Code, string Barcode, int Level, int Row, int Column);

public sealed record UpdateLocationRequest(Guid StructureId, string Code, string Barcode, int Level, int Row, int Column);
