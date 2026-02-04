namespace DevcraftWMS.Api.Contracts;

public sealed record ConvertAsnToInboundOrderRequest(Guid AsnId, string? Notes);

public sealed record UpdateInboundOrderParametersRequest(
    int InspectionLevel,
    int Priority,
    string? SuggestedDock);

public sealed record CancelInboundOrderRequest(string Reason);

public sealed record CompleteInboundOrderRequest(bool AllowPartial, string? Notes);
