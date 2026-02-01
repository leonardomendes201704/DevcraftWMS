using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Emails.Queries.GetEmailById;

public sealed record GetEmailByIdQuery(Guid Id) : IRequest<RequestResult<EmailMessageDto>>;

