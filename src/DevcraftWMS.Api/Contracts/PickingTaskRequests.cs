namespace DevcraftWMS.Api.Contracts;

public sealed record ConfirmPickingTaskItemRequest(
    Guid PickingTaskItemId,
    decimal QuantityPicked);

public sealed record ConfirmPickingTaskRequest(
    IReadOnlyList<ConfirmPickingTaskItemRequest> Items,
    string? Notes);
