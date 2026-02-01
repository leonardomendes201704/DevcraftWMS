namespace PerfectApiTemplate.Api.Contracts;

public sealed record CreateCustomerRequest(string Name, string Email, DateOnly DateOfBirth);

public sealed record UpdateCustomerRequest(string Name, string Email, DateOnly DateOfBirth);

