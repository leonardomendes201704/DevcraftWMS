using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Commands.ResetUserPassword;

public sealed record ResetUserPasswordCommand(Guid Id) : IRequest<RequestResult<string>>;
