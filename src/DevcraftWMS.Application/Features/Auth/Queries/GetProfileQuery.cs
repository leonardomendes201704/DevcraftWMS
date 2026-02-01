using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Auth.Queries;

public sealed record GetProfileQuery : IRequest<RequestResult<UserProfileDto>>;

public sealed record UserProfileDto(Guid Id, string Email, string FullName, bool IsActive, DateTime CreatedAtUtc, DateTime? LastLoginUtc);

