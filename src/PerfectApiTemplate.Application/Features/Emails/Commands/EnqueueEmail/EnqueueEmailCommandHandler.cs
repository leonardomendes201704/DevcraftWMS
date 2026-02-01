using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Emails.Commands.EnqueueEmail;

public sealed class EnqueueEmailCommandHandler : IRequestHandler<EnqueueEmailCommand, RequestResult<EmailMessageDto>>
{
    private readonly IEmailService _emailService;

    public EnqueueEmailCommandHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task<RequestResult<EmailMessageDto>> Handle(EnqueueEmailCommand request, CancellationToken cancellationToken)
        => _emailService.EnqueueAsync(request.From, request.To, request.Subject, request.Body, request.IsHtml, cancellationToken);
}

