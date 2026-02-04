namespace DevcraftWMS.Api.Contracts;

public sealed record ConfirmPutawayTaskRequest(
    Guid LocationId,
    string? Notes);
