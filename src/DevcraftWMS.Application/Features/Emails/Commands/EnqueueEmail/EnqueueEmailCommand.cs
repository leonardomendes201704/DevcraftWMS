using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Emails.Commands.EnqueueEmail;

public sealed record EnqueueEmailCommand(
    string? From,
    string To,
    string Subject,
    string Body,
    bool IsHtml) : IRequest<RequestResult<EmailMessageDto>>;


