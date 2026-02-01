using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Emails.Queries.GetEmailById;

public sealed class GetEmailByIdQueryHandler : IRequestHandler<GetEmailByIdQuery, RequestResult<EmailMessageDto>>
{
    private readonly IEmailService _emailService;

    public GetEmailByIdQueryHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task<RequestResult<EmailMessageDto>> Handle(GetEmailByIdQuery request, CancellationToken cancellationToken)
        => _emailService.GetByIdAsync(request.Id, cancellationToken);
}

