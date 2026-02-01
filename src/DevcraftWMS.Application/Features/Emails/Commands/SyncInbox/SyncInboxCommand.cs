using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Emails.Commands.SyncInbox;

public sealed record SyncInboxCommand(int MaxMessages = 50) : IRequest<RequestResult<int>>;


