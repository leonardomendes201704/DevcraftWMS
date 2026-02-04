namespace DevcraftWMS.Api.Contracts;

public sealed record ConfirmPutawayTaskRequest(
    Guid LocationId,
    string? Notes);

public sealed record ReassignPutawayTaskRequest(
    string AssigneeEmail,
    string Reason);
