using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Emails.Commands.SyncInbox;

public sealed record SyncInboxCommand(int MaxMessages = 50) : IRequest<RequestResult<int>>;

