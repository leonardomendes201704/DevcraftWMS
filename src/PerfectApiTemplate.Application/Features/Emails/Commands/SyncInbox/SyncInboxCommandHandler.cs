using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Emails.Commands.SyncInbox;

public sealed class SyncInboxCommandHandler : IRequestHandler<SyncInboxCommand, RequestResult<int>>
{
    private readonly IEmailService _emailService;

    public SyncInboxCommandHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task<RequestResult<int>> Handle(SyncInboxCommand request, CancellationToken cancellationToken)
        => _emailService.SyncInboxAsync(request.MaxMessages, cancellationToken);
}

