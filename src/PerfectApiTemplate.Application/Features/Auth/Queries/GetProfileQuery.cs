using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Auth.Queries;

public sealed record GetProfileQuery : IRequest<RequestResult<UserProfileDto>>;

public sealed record UserProfileDto(Guid Id, string Email, string FullName, bool IsActive, DateTime CreatedAtUtc, DateTime? LastLoginUtc);
