namespace PerfectApiTemplate.Application.Features.Customers;

public sealed record CustomerDto(Guid Id, string Name, string Email, DateOnly DateOfBirth, DateTime CreatedAtUtc);

