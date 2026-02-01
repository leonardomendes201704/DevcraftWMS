using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Emails.Queries.GetEmailById;

public sealed record GetEmailByIdQuery(Guid Id) : IRequest<RequestResult<EmailMessageDto>>;


